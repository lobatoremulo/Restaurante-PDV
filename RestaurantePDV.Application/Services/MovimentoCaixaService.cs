using RestaurantePDV.Application.DTOs;
using RestaurantePDV.Application.Interfaces;
using RestaurantePDV.Domain.Entities;
using RestaurantePDV.Domain.Enums;

namespace RestaurantePDV.Application.Services;

public class MovimentoCaixaService : IMovimentoCaixaService
{
    private readonly IMovimentoCaixaRepository _movimentoCaixaRepository;
    private readonly ICaixaRepository _caixaRepository;
    private readonly IVendaRepository _vendaRepository;
    private readonly IFuncionarioRepository _funcionarioRepository;

    public MovimentoCaixaService(
        IMovimentoCaixaRepository movimentoCaixaRepository,
        ICaixaRepository caixaRepository,
        IVendaRepository vendaRepository,
        IFuncionarioRepository funcionarioRepository)
    {
        _movimentoCaixaRepository = movimentoCaixaRepository;
        _caixaRepository = caixaRepository;
        _vendaRepository = vendaRepository;
        _funcionarioRepository = funcionarioRepository;
    }

    public async Task<MovimentoCaixaDto> AdicionarMovimentoAsync(MovimentoCaixaCreateDto movimentoDto)
    {
        // Validações
        var caixa = await _caixaRepository.GetByIdAsync(movimentoDto.CaixaId);
        if (caixa == null)
            throw new ArgumentException("Caixa não encontrado");

        if (caixa.Status != StatusCaixa.Aberto)
            throw new InvalidOperationException("Não é possível adicionar movimento em caixa fechado");

        var operador = await _funcionarioRepository.GetByIdAsync(movimentoDto.OperadorId);
        if (operador == null)
            throw new ArgumentException("Operador não encontrado");

        // Validar venda se informada
        if (movimentoDto.VendaId.HasValue)
        {
            var venda = await _vendaRepository.GetByIdAsync(movimentoDto.VendaId.Value);
            if (venda == null)
                throw new ArgumentException("Venda não encontrada");
        }

        var movimento = new MovimentoCaixa
        {
            CaixaId = movimentoDto.CaixaId,
            TipoMovimento = movimentoDto.TipoMovimento,
            Valor = movimentoDto.Valor,
            Descricao = movimentoDto.Descricao,
            Observacoes = movimentoDto.Observacoes,
            FormaPagamento = movimentoDto.FormaPagamento,
            VendaId = movimentoDto.VendaId,
            OperadorId = movimentoDto.OperadorId,
            NumeroDocumento = movimentoDto.NumeroDocumento,
            DataMovimento = DateTime.UtcNow
        };

        await _movimentoCaixaRepository.AddAsync(movimento);

        // Atualizar totais do caixa
        await AtualizarTotaisCaixa(movimentoDto.CaixaId);

        return await MapToMovimentoCaixaDto(movimento);
    }

    public async Task<MovimentoCaixaDto> RegistrarVendaAsync(int vendaId, int caixaId, int operadorId)
    {
        var venda = await _vendaRepository.GetByIdAsync(vendaId);
        if (venda == null)
            throw new ArgumentException("Venda não encontrada");

        // Verificar se a venda já foi registrada no caixa
        var movimentos = await _movimentoCaixaRepository.GetByCaixaAsync(caixaId);
        if (movimentos.Any(m => m.VendaId == vendaId))
            throw new InvalidOperationException("Esta venda já foi registrada no caixa");

        var movimentoDto = new MovimentoCaixaCreateDto
        {
            CaixaId = caixaId,
            TipoMovimento = TipoMovimentoCaixa.Venda,
            Valor = venda.ValorTotal,
            Descricao = $"Venda #{venda.NumeroVenda}",
            FormaPagamento = venda.FormaPagamento,
            VendaId = vendaId,
            OperadorId = operadorId
        };

        return await AdicionarMovimentoAsync(movimentoDto);
    }

    public async Task<MovimentoCaixaDto> RegistrarSangriaAsync(MovimentoCaixaCreateDto sangriaDto)
    {
        sangriaDto.TipoMovimento = TipoMovimentoCaixa.Sangria;
        
        if (string.IsNullOrEmpty(sangriaDto.Descricao))
            sangriaDto.Descricao = "Sangria de caixa";

        return await AdicionarMovimentoAsync(sangriaDto);
    }

    public async Task<MovimentoCaixaDto> RegistrarSuprimentoAsync(MovimentoCaixaCreateDto suprimentoDto)
    {
        suprimentoDto.TipoMovimento = TipoMovimentoCaixa.Suprimento;
        
        if (string.IsNullOrEmpty(suprimentoDto.Descricao))
            suprimentoDto.Descricao = "Suprimento de caixa";

        return await AdicionarMovimentoAsync(suprimentoDto);
    }

    public async Task<IEnumerable<MovimentoCaixaDto>> GetByCaixaAsync(int caixaId)
    {
        var movimentos = await _movimentoCaixaRepository.GetByCaixaAsync(caixaId);
        var tasks = movimentos.Select(async m => await MapToMovimentoCaixaDto(m));
        return await Task.WhenAll(tasks);
    }

    public async Task<IEnumerable<MovimentoCaixaDto>> GetByPeriodoAsync(DateTime dataInicio, DateTime dataFim)
    {
        var movimentos = await _movimentoCaixaRepository.GetByPeriodoAsync(dataInicio, dataFim);
        var tasks = movimentos.Select(async m => await MapToMovimentoCaixaDto(m));
        return await Task.WhenAll(tasks);
    }

    public async Task<MovimentoCaixaDto> GetByIdAsync(int id)
    {
        var movimento = await _movimentoCaixaRepository.GetByIdAsync(id);
        if (movimento == null)
            throw new ArgumentException("Movimento não encontrado");

        return await MapToMovimentoCaixaDto(movimento);
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

    private async Task<MovimentoCaixaDto> MapToMovimentoCaixaDto(MovimentoCaixa movimento)
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