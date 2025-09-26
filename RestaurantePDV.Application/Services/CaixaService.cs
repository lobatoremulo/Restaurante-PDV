using RestaurantePDV.Application.DTOs;
using RestaurantePDV.Application.Interfaces;
using RestaurantePDV.Domain.Entities;
using RestaurantePDV.Domain.Enums;

namespace RestaurantePDV.Application.Services;

public interface ICaixaService
{
    Task<CaixaDto> AbrirCaixaAsync(CaixaCreateDto caixaDto);
    Task<CaixaDto> FecharCaixaAsync(CaixaFechamentoDto fechamentoDto);
    Task<CaixaDto?> GetCaixaAbertoAsync();
    Task<CaixaDto> GetByIdAsync(int id);
    Task<IEnumerable<CaixaListDto>> GetAllAsync();
    Task<IEnumerable<CaixaListDto>> GetByPeriodoAsync(DateTime dataInicio, DateTime dataFim);
    Task<bool> TemCaixaAbertoAsync();
    Task<object> GetRelatorioCaixaAsync(int caixaId);
    Task<object> GetRelatorioFinanceiroAsync(DateTime dataInicio, DateTime dataFim);
}

public interface IMovimentoCaixaService
{
    Task<MovimentoCaixaDto> AdicionarMovimentoAsync(MovimentoCaixaCreateDto movimentoDto);
    Task<MovimentoCaixaDto> RegistrarVendaAsync(int vendaId, int caixaId, int operadorId);
    Task<MovimentoCaixaDto> RegistrarSangriaAsync(MovimentoCaixaCreateDto sangriaDto);
    Task<MovimentoCaixaDto> RegistrarSuprimentoAsync(MovimentoCaixaCreateDto suprimentoDto);
    Task<IEnumerable<MovimentoCaixaDto>> GetByCaixaAsync(int caixaId);
    Task<IEnumerable<MovimentoCaixaDto>> GetByPeriodoAsync(DateTime dataInicio, DateTime dataFim);
    Task<MovimentoCaixaDto> GetByIdAsync(int id);
}

public class CaixaService : ICaixaService
{
    private readonly ICaixaRepository _caixaRepository;
    private readonly IMovimentoCaixaRepository _movimentoCaixaRepository;
    private readonly IFuncionarioRepository _funcionarioRepository;

    public CaixaService(
        ICaixaRepository caixaRepository,
        IMovimentoCaixaRepository movimentoCaixaRepository,
        IFuncionarioRepository funcionarioRepository)
    {
        _caixaRepository = caixaRepository;
        _movimentoCaixaRepository = movimentoCaixaRepository;
        _funcionarioRepository = funcionarioRepository;
    }

    public async Task<CaixaDto> AbrirCaixaAsync(CaixaCreateDto caixaDto)
    {
        // Verificar se já existe caixa aberto
        if (await _caixaRepository.TemCaixaAbertoAsync())
            throw new InvalidOperationException("Já existe um caixa aberto. Feche o caixa atual antes de abrir um novo.");

        // Verificar se o operador existe
        var operador = await _funcionarioRepository.GetByIdAsync(caixaDto.OperadorAberturaId);
        if (operador == null)
            throw new ArgumentException("Operador não encontrado");

        var caixa = new Caixa
        {
            DataAbertura = DateTime.UtcNow,
            ValorAbertura = caixaDto.ValorAbertura,
            ObservacoesAbertura = caixaDto.ObservacoesAbertura,
            OperadorAberturaId = caixaDto.OperadorAberturaId,
            Status = StatusCaixa.Aberto
        };

        await _caixaRepository.AddAsync(caixa);

        // Registrar movimento de abertura
        if (caixaDto.ValorAbertura > 0)
        {
            var movimentoAbertura = new MovimentoCaixa
            {
                CaixaId = caixa.Id,
                TipoMovimento = TipoMovimentoCaixa.Abertura,
                Valor = caixaDto.ValorAbertura,
                Descricao = "Abertura de caixa",
                Observacoes = caixaDto.ObservacoesAbertura,
                OperadorId = caixaDto.OperadorAberturaId,
                FormaPagamento = FormaPagamento.Dinheiro
            };

            await _movimentoCaixaRepository.AddAsync(movimentoAbertura);
        }

        return await MapToCaixaDto(caixa);
    }

    public async Task<CaixaDto> FecharCaixaAsync(CaixaFechamentoDto fechamentoDto)
    {
        var caixa = await _caixaRepository.GetByIdAsync(fechamentoDto.CaixaId);
        if (caixa == null)
            throw new ArgumentException("Caixa não encontrado");

        if (caixa.Status != StatusCaixa.Aberto)
            throw new InvalidOperationException("Apenas caixas abertos podem ser fechados");

        // Verificar se o operador existe
        var operador = await _funcionarioRepository.GetByIdAsync(fechamentoDto.OperadorFechamentoId);
        if (operador == null)
            throw new ArgumentException("Operador não encontrado");

        // Atualizar totais do caixa
        await AtualizarTotaisCaixa(caixa.Id);

        caixa.DataFechamento = DateTime.UtcNow;
        caixa.ValorFechamento = fechamentoDto.ValorFechamento;
        caixa.ObservacoesFechamento = fechamentoDto.ObservacoesFechamento;
        caixa.OperadorFechamentoId = fechamentoDto.OperadorFechamentoId;
        caixa.Status = StatusCaixa.Fechado;

        await _caixaRepository.UpdateAsync(caixa);

        // Registrar movimento de fechamento se houver diferença
        var saldoTeorico = caixa.ValorAbertura + caixa.TotalVendas + caixa.TotalSuprimentos - caixa.TotalSangrias;
        var diferenca = fechamentoDto.ValorFechamento - saldoTeorico;

        if (Math.Abs(diferenca) > 0.01m)
        {
            var movimentoFechamento = new MovimentoCaixa
            {
                CaixaId = caixa.Id,
                TipoMovimento = TipoMovimentoCaixa.Fechamento,
                Valor = Math.Abs(diferenca),
                Descricao = diferenca > 0 ? "Sobra de caixa" : "Falta de caixa",
                Observacoes = fechamentoDto.ObservacoesFechamento,
                OperadorId = fechamentoDto.OperadorFechamentoId
            };

            await _movimentoCaixaRepository.AddAsync(movimentoFechamento);
        }

        return await MapToCaixaDto(caixa);
    }

    public async Task<CaixaDto?> GetCaixaAbertoAsync()
    {
        var caixa = await _caixaRepository.GetCaixaAbertoAsync();
        return caixa != null ? await MapToCaixaDto(caixa) : null;
    }

    public async Task<CaixaDto> GetByIdAsync(int id)
    {
        var caixa = await _caixaRepository.GetByIdAsync(id);
        if (caixa == null)
            throw new ArgumentException("Caixa não encontrado");

        return await MapToCaixaDto(caixa);
    }

    public async Task<IEnumerable<CaixaListDto>> GetAllAsync()
    {
        var caixas = await _caixaRepository.GetAllAsync();
        return caixas.Select(MapToCaixaListDto);
    }

    public async Task<IEnumerable<CaixaListDto>> GetByPeriodoAsync(DateTime dataInicio, DateTime dataFim)
    {
        var caixas = await _caixaRepository.GetByPeriodoAsync(dataInicio, dataFim);
        return caixas.Select(MapToCaixaListDto);
    }

    public async Task<bool> TemCaixaAbertoAsync()
    {
        return await _caixaRepository.TemCaixaAbertoAsync();
    }

    public async Task<object> GetRelatorioCaixaAsync(int caixaId)
    {
        var caixa = await _caixaRepository.GetWithMovimentosAsync(caixaId);
        if (caixa == null)
            throw new ArgumentException("Caixa não encontrado");

        var movimentos = await _movimentoCaixaRepository.GetByCaixaAsync(caixaId);

        return new
        {
            Caixa = await MapToCaixaDto(caixa),
            Movimentos = movimentos.Select(MapToMovimentoCaixaDto),
            Resumo = new
            {
                TotalVendas = movimentos.Where(m => m.TipoMovimento == TipoMovimentoCaixa.Venda).Sum(m => m.Valor),
                TotalSangrias = movimentos.Where(m => m.TipoMovimento == TipoMovimentoCaixa.Sangria).Sum(m => m.Valor),
                TotalSuprimentos = movimentos.Where(m => m.TipoMovimento == TipoMovimentoCaixa.Suprimento).Sum(m => m.Valor),
                QuantidadeMovimentos = movimentos.Count(),
                PorFormaPagamento = movimentos
                    .Where(m => m.FormaPagamento.HasValue)
                    .GroupBy(m => m.FormaPagamento)
                    .Select(g => new { FormaPagamento = g.Key.ToString(), Total = g.Sum(m => m.Valor) })
            }
        };
    }

    public async Task<object> GetRelatorioFinanceiroAsync(DateTime dataInicio, DateTime dataFim)
    {
        var caixas = await _caixaRepository.GetByPeriodoAsync(dataInicio, dataFim);
        var movimentos = await _movimentoCaixaRepository.GetByPeriodoAsync(dataInicio, dataFim);

        return new
        {
            Periodo = new { DataInicio = dataInicio, DataFim = dataFim },
            TotalCaixas = caixas.Count(),
            CaixasFechados = caixas.Count(c => c.Status == StatusCaixa.Fechado),
            CaixasAbertos = caixas.Count(c => c.Status == StatusCaixa.Aberto),
            TotalVendas = movimentos.Where(m => m.TipoMovimento == TipoMovimentoCaixa.Venda).Sum(m => m.Valor),
            TotalSangrias = movimentos.Where(m => m.TipoMovimento == TipoMovimentoCaixa.Sangria).Sum(m => m.Valor),
            TotalSuprimentos = movimentos.Where(m => m.TipoMovimento == TipoMovimentoCaixa.Suprimento).Sum(m => m.Valor),
            PorFormaPagamento = movimentos
                .Where(m => m.FormaPagamento.HasValue)
                .GroupBy(m => m.FormaPagamento)
                .Select(g => new { FormaPagamento = g.Key.ToString(), Total = g.Sum(m => m.Valor) }),
            CaixasPorDia = caixas
                .GroupBy(c => c.DataAbertura.Date)
                .Select(g => new 
                { 
                    Data = g.Key, 
                    Quantidade = g.Count(),
                    TotalVendas = g.Sum(c => c.TotalVendas)
                })
                .OrderBy(x => x.Data)
        };
    }

    private async Task AtualizarTotaisCaixa(int caixaId)
    {
        var caixa = await _caixaRepository.GetByIdAsync(caixaId);
        if (caixa == null) return;

        caixa.TotalVendas = await _movimentoCaixaRepository.GetTotalByTipoAsync(caixaId, TipoMovimentoCaixa.Venda);
        caixa.TotalSangrias = await _movimentoCaixaRepository.GetTotalByTipoAsync(caixaId, TipoMovimentoCaixa.Sangria);
        caixa.TotalSuprimentos = await _movimentoCaixaRepository.GetTotalByTipoAsync(caixaId, TipoMovimentoCaixa.Suprimento);

        await _caixaRepository.UpdateAsync(caixa);
    }

    private async Task<CaixaDto> MapToCaixaDto(Caixa caixa)
    {
        return new CaixaDto
        {
            Id = caixa.Id,
            DataAbertura = caixa.DataAbertura,
            DataFechamento = caixa.DataFechamento,
            Status = caixa.Status,
            ValorAbertura = caixa.ValorAbertura,
            ValorFechamento = caixa.ValorFechamento,
            TotalVendas = caixa.TotalVendas,
            TotalSangrias = caixa.TotalSangrias,
            TotalSuprimentos = caixa.TotalSuprimentos,
            ObservacoesAbertura = caixa.ObservacoesAbertura,
            ObservacoesFechamento = caixa.ObservacoesFechamento,
            OperadorAberturaId = caixa.OperadorAberturaId,
            OperadorAberturaNome = caixa.OperadorAbertura?.Nome ?? "",
            OperadorFechamentoId = caixa.OperadorFechamentoId,
            OperadorFechamentoNome = caixa.OperadorFechamento?.Nome,
            CriadoEm = caixa.CriadoEm
        };
    }

    private static CaixaListDto MapToCaixaListDto(Caixa caixa)
    {
        return new CaixaListDto
        {
            Id = caixa.Id,
            DataAbertura = caixa.DataAbertura,
            DataFechamento = caixa.DataFechamento,
            Status = caixa.Status,
            ValorAbertura = caixa.ValorAbertura,
            ValorFechamento = caixa.ValorFechamento,
            TotalVendas = caixa.TotalVendas,
            TotalSangrias = caixa.TotalSangrias,
            TotalSuprimentos = caixa.TotalSuprimentos,
            OperadorAberturaNome = caixa.OperadorAbertura?.Nome ?? "",
            OperadorFechamentoNome = caixa.OperadorFechamento?.Nome
        };
    }

    private static MovimentoCaixaDto MapToMovimentoCaixaDto(MovimentoCaixa movimento)
    {
        return new MovimentoCaixaDto
        {
            Id = movimento.Id,
            CaixaId = movimento.CaixaId,
            DataMovimento = movimento.DataMovimento,
            TipoMovimento = movimento.TipoMovimento,
            Valor = movimento.Valor,
            Descricao = movimento.Descricao,
            Observacoes = movimento.Observacoes,
            FormaPagamento = movimento.FormaPagamento,
            VendaId = movimento.VendaId,
            VendaNumero = movimento.Venda?.NumeroVenda,
            OperadorId = movimento.OperadorId,
            OperadorNome = movimento.Operador?.Nome ?? "",
            NumeroDocumento = movimento.NumeroDocumento,
            CriadoEm = movimento.CriadoEm
        };
    }
}