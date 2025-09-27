using RestaurantePDV.Application.DTOs;
using RestaurantePDV.Application.Interfaces;
using RestaurantePDV.Domain.Entities;
using RestaurantePDV.Domain.Enums;

namespace RestaurantePDV.Application.Services;

public interface IVendaService
{
    // Métodos existentes
    Task<VendaDto> CriarVendaAsync(VendaCreateDto vendaDto);
    Task<VendaDto> FinalizarVendaAsync(int vendaId, FormaPagamento formaPagamento, decimal valorPago);
    Task<VendaDto> CancelarVendaAsync(int vendaId);
    Task<VendaDto> GetByIdAsync(int id);
    Task<VendaDto?> GetByNumeroAsync(string numeroVenda);
    Task<IEnumerable<VendaDto>> GetByPeriodoAsync(DateTime dataInicio, DateTime dataFim);
    Task<decimal> GetTotalVendasPeriodoAsync(DateTime dataInicio, DateTime dataFim);
    
    // Novos métodos para pagamento de comandas
    Task<ComandaParaPagamentoDto> PrepararPagamentoComandaAsync(int comandaId);
    Task<VendaComandaDto> ProcessarPagamentoComandaAsync(PagamentoComandaDto pagamentoDto);
    Task<VendaComandaDto> GetVendaComandaAsync(int vendaId);
    Task<object> ValidarPagamentoComandaAsync(PagamentoComandaDto pagamentoDto);
    Task<IEnumerable<ComandaParaPagamentoDto>> GetComandasPendentesAsync();
    Task<VendaComandaDto> ReprocessarPagamentoAsync(int vendaId, PagamentoComandaDto pagamentoDto);
    Task<object> GetRelatorioVendasAsync(DateTime dataInicio, DateTime dataFim);
}

public class VendaService : IVendaService
{
    private readonly IVendaRepository _vendaRepository;
    private readonly IProdutoRepository _produtoRepository;
    private readonly IClienteRepository _clienteRepository;
    private readonly IComandaRepository _comandaRepository;
    private readonly ICaixaService _caixaService;
    private readonly IMovimentoCaixaService _movimentoCaixaService;

    public VendaService(
        IVendaRepository vendaRepository,
        IProdutoRepository produtoRepository,
        IClienteRepository clienteRepository,
        IComandaRepository comandaRepository,
        ICaixaService caixaService,
        IMovimentoCaixaService movimentoCaixaService)
    {
        _vendaRepository = vendaRepository;
        _produtoRepository = produtoRepository;
        _clienteRepository = clienteRepository;
        _comandaRepository = comandaRepository;
        _caixaService = caixaService;
        _movimentoCaixaService = movimentoCaixaService;
    }

    public async Task<VendaDto> CriarVendaAsync(VendaCreateDto vendaDto)
    {
        if (!vendaDto.Itens.Any())
            throw new ArgumentException("Venda deve ter pelo menos um item");

        var numeroVenda = await GerarNumeroVendaAsync();

        var venda = new Venda
        {
            NumeroVenda = numeroVenda,
            ClienteId = vendaDto.ClienteId,
            DataVenda = DateTime.UtcNow,
            Status = StatusVenda.Aberta,
            FormaPagamento = vendaDto.FormaPagamento,
            Desconto = vendaDto.Desconto,
            Acrescimo = vendaDto.Acrescimo,
            Observacoes = vendaDto.Observacoes,
            VendaBalcao = vendaDto.VendaBalcao,
            ComandaId = vendaDto.ComandaId
        };

        // Processar itens
        foreach (var itemDto in vendaDto.Itens)
        {
            var produto = await _produtoRepository.GetByIdAsync(itemDto.ProdutoId);
            if (produto == null)
                throw new ArgumentException($"Produto {itemDto.ProdutoId} não encontrado");

            var valorUnitario = produto.PrecoVenda;
            var valorTotalItem = (valorUnitario * itemDto.Quantidade) - itemDto.Desconto;

            var item = new ItemVenda
            {
                ProdutoId = itemDto.ProdutoId,
                Quantidade = itemDto.Quantidade,
                ValorUnitario = valorUnitario,
                Desconto = itemDto.Desconto,
                ValorTotal = valorTotalItem,
                Observacoes = itemDto.Observacoes,
                Adicionais = itemDto.Adicionais
            };

            venda.Itens.Add(item);
        }

        // Calcular totais
        venda.SubTotal = venda.Itens.Sum(i => i.ValorTotal);
        venda.ValorTotal = venda.SubTotal - venda.Desconto + venda.Acrescimo;

        var vendaCriada = await _vendaRepository.AddAsync(venda);
        return await MapToDtoAsync(vendaCriada);
    }

    public async Task<VendaDto> FinalizarVendaAsync(int vendaId, FormaPagamento formaPagamento, decimal valorPago)
    {
        var venda = await _vendaRepository.GetComItensAsync(vendaId);
        if (venda == null)
            throw new ArgumentException("Venda não encontrada");

        if (venda.Status != StatusVenda.Aberta)
            throw new InvalidOperationException("Venda não está aberta");

        if (valorPago < venda.ValorTotal)
            throw new ArgumentException("Valor pago insuficiente");

        venda.Status = StatusVenda.Finalizada;
        venda.FormaPagamento = formaPagamento;
        venda.ValorPago = valorPago;
        venda.Troco = valorPago - venda.ValorTotal;

        // Aqui seria feita a baixa no estoque
        await ProcessarBaixaEstoqueAsync(venda);

        var vendaAtualizada = await _vendaRepository.UpdateAsync(venda);
        return await MapToDtoAsync(vendaAtualizada);
    }

    public async Task<VendaDto> CancelarVendaAsync(int vendaId)
    {
        var venda = await _vendaRepository.GetByIdAsync(vendaId);
        if (venda == null)
            throw new ArgumentException("Venda não encontrada");

        if (venda.Status == StatusVenda.Finalizada)
            throw new InvalidOperationException("Venda já finalizada não pode ser cancelada");

        venda.Status = StatusVenda.Cancelada;

        var vendaAtualizada = await _vendaRepository.UpdateAsync(venda);
        return await MapToDtoAsync(vendaAtualizada);
    }

    public async Task<VendaDto> GetByIdAsync(int id)
    {
        var venda = await _vendaRepository.GetComItensAsync(id);
        if (venda == null)
            throw new ArgumentException("Venda não encontrada");

        return await MapToDtoAsync(venda);
    }

    public async Task<VendaDto?> GetByNumeroAsync(string numeroVenda)
    {
        var venda = await _vendaRepository.GetByNumeroAsync(numeroVenda);
        return venda != null ? await MapToDtoAsync(venda) : null;
    }

    public async Task<IEnumerable<VendaDto>> GetByPeriodoAsync(DateTime dataInicio, DateTime dataFim)
    {
        var vendas = await _vendaRepository.GetByPeriodoAsync(dataInicio, dataFim);
        var vendaDtos = new List<VendaDto>();

        foreach (var venda in vendas)
        {
            vendaDtos.Add(await MapToDtoAsync(venda));
        }

        return vendaDtos;
    }

    public async Task<decimal> GetTotalVendasPeriodoAsync(DateTime dataInicio, DateTime dataFim)
    {
        return await _vendaRepository.GetTotalVendasPeriodoAsync(dataInicio, dataFim);
    }

    // NOVOS MÉTODOS PARA PAGAMENTO DE COMANDAS

    public async Task<ComandaParaPagamentoDto> PrepararPagamentoComandaAsync(int comandaId)
    {
        var comanda = await _comandaRepository.GetComItensAsync(comandaId);
        if (comanda == null)
            throw new ArgumentException("Comanda não encontrada");

        if (comanda.Status != StatusComanda.Fechada)
            throw new InvalidOperationException("Apenas comandas fechadas podem ser pagas");

        return new ComandaParaPagamentoDto
        {
            Id = comanda.Id,
            NumeroComanda = comanda.NumeroComanda,
            Mesa = comanda.Mesa,
            ClienteNome = comanda.Cliente?.Nome,
            GarcomNome = comanda.Garcom?.Nome,
            ValorTotal = comanda.ValorTotal,
            Desconto = comanda.Desconto,
            Acrescimo = comanda.Acrescimo,
            ValorFinal = comanda.ValorFinal,
            DataAbertura = comanda.DataAbertura,
            Itens = comanda.Itens?.Select(i => new ItemComandaResumoDto
            {
                Id = i.Id,
                ProdutoNome = i.Produto?.Nome ?? "",
                Quantidade = i.Quantidade,
                ValorUnitario = i.ValorUnitario,
                ValorTotal = i.ValorTotal,
                Observacoes = i.Observacoes,
                Adicionais = i.Adicionais
            }).ToList() ?? new List<ItemComandaResumoDto>()
        };
    }

    public async Task<VendaComandaDto> ProcessarPagamentoComandaAsync(PagamentoComandaDto pagamentoDto)
    {
        // Validar se existe caixa aberto
        var caixaAberto = await _caixaService.GetCaixaAbertoAsync();
        if (caixaAberto == null)
            throw new InvalidOperationException("Não há caixa aberto para processar a venda");

        var comanda = await _comandaRepository.GetComItensAsync(pagamentoDto.ComandaId);
        if (comanda == null)
            throw new ArgumentException("Comanda não encontrada");

        if (comanda.Status != StatusComanda.Fechada)
            throw new InvalidOperationException("Apenas comandas fechadas podem ser pagas");

        // Validar valores dos pagamentos
        var totalPagamentos = pagamentoDto.FormasPagamento.Sum(fp => fp.Valor);
        var valorFinal = comanda.ValorFinal - pagamentoDto.Desconto + pagamentoDto.Acrescimo;

        if (totalPagamentos < valorFinal)
            throw new ArgumentException("Valor total dos pagamentos é insuficiente");

        // Criar a venda
        var numeroVenda = await GerarNumeroVendaAsync();
        var venda = new Venda
        {
            NumeroVenda = numeroVenda,
            ComandaId = comanda.Id,
            ClienteId = comanda.ClienteId,
            DataVenda = DateTime.UtcNow,
            Status = StatusVenda.Finalizada,
            SubTotal = comanda.ValorTotal,
            Desconto = pagamentoDto.Desconto,
            Acrescimo = pagamentoDto.Acrescimo,
            ValorTotal = valorFinal,
            ValorPago = totalPagamentos,
            Troco = totalPagamentos - valorFinal,
            Observacoes = pagamentoDto.Observacoes,
            VendaBalcao = false
        };

        // Copiar itens da comanda para a venda
        if (comanda.Itens != null)
        {
            foreach (var itemComanda in comanda.Itens)
            {
                var itemVenda = new ItemVenda
                {
                    ProdutoId = itemComanda.ProdutoId,
                    Quantidade = itemComanda.Quantidade,
                    ValorUnitario = itemComanda.ValorUnitario,
                    ValorTotal = itemComanda.ValorTotal,
                    Observacoes = itemComanda.Observacoes,
                    Adicionais = itemComanda.Adicionais
                };
                venda.Itens.Add(itemVenda);
            }
        }

        // Adicionar pagamentos
        foreach (var formaPagamento in pagamentoDto.FormasPagamento)
        {
            var pagamento = new PagamentoVenda
            {
                FormaPagamento = formaPagamento.FormaPagamento,
                Valor = formaPagamento.Valor,
                ValorRecebido = formaPagamento.ValorRecebido,
                NumeroDocumento = formaPagamento.NumeroDocumento,
                Observacoes = formaPagamento.Observacoes,
                DataPagamento = DateTime.UtcNow
            };
            venda.Pagamentos.Add(pagamento);
        }

        // Salvar a venda
        var vendaCriada = await _vendaRepository.AddAsync(venda);

        // Processar baixa no estoque
        await ProcessarBaixaEstoqueAsync(vendaCriada);

        // Criar movimentos no caixa para cada forma de pagamento
        foreach (var formaPagamento in pagamentoDto.FormasPagamento)
        {
            var movimentoCaixa = new MovimentoCaixaCreateDto
            {
                CaixaId = caixaAberto.Id,
                TipoMovimento = TipoMovimentoCaixa.Venda,
                Valor = formaPagamento.Valor,
                Descricao = $"Venda {numeroVenda} - Comanda {comanda.NumeroComanda}",
                Observacoes = formaPagamento.Observacoes,
                FormaPagamento = formaPagamento.FormaPagamento,
                VendaId = vendaCriada.Id,
                OperadorId = pagamentoDto.OperadorId,
                NumeroDocumento = formaPagamento.NumeroDocumento
            };

            await _movimentoCaixaService.AdicionarMovimentoAsync(movimentoCaixa);
        }

        return await MapToVendaComandaDtoAsync(vendaCriada, pagamentoDto.FormasPagamento);
    }

    public async Task<VendaComandaDto> GetVendaComandaAsync(int vendaId)
    {
        var venda = await _vendaRepository.GetComItensAsync(vendaId);
        if (venda == null)
            throw new ArgumentException("Venda não encontrada");

        // Buscar os pagamentos da venda
        var formasPagamento = venda.Pagamentos?.Select(p => new FormaPagamentoDto
        {
            FormaPagamento = p.FormaPagamento,
            Valor = p.Valor,
            ValorRecebido = p.ValorRecebido,
            NumeroDocumento = p.NumeroDocumento,
            Observacoes = p.Observacoes
        }).ToList() ?? new List<FormaPagamentoDto>();

        return await MapToVendaComandaDtoAsync(venda, formasPagamento);
    }

    public async Task<object> ValidarPagamentoComandaAsync(PagamentoComandaDto pagamentoDto)
    {
        var comanda = await _comandaRepository.GetByIdAsync(pagamentoDto.ComandaId);
        if (comanda == null)
            return new { valido = false, erro = "Comanda não encontrada" };

        if (comanda.Status != StatusComanda.Fechada)
            return new { valido = false, erro = "Comanda deve estar fechada para pagamento" };

        var totalPagamentos = pagamentoDto.FormasPagamento.Sum(fp => fp.Valor);
        var valorFinal = comanda.ValorFinal - pagamentoDto.Desconto + pagamentoDto.Acrescimo;

        if (totalPagamentos < valorFinal)
            return new { valido = false, erro = "Valor insuficiente", valorNecessario = valorFinal, valorInformado = totalPagamentos };

        var troco = totalPagamentos - valorFinal;
        return new { valido = true, troco = troco, valorFinal = valorFinal };
    }

    public async Task<IEnumerable<ComandaParaPagamentoDto>> GetComandasPendentesAsync()
    {
        var comandas = await _comandaRepository.GetByStatusAsync(StatusComanda.Fechada);
        
        var comandasPendentes = new List<ComandaParaPagamentoDto>();
        foreach (var comanda in comandas)
        {
            // Verificar se já foi paga
            var jaFoiPaga = await _vendaRepository.ExisteVendaParaComandaAsync(comanda.Id);
            if (!jaFoiPaga)
            {
                comandasPendentes.Add(new ComandaParaPagamentoDto
                {
                    Id = comanda.Id,
                    NumeroComanda = comanda.NumeroComanda,
                    Mesa = comanda.Mesa,
                    ClienteNome = comanda.Cliente?.Nome,
                    GarcomNome = comanda.Garcom?.Nome,
                    ValorTotal = comanda.ValorTotal,
                    Desconto = comanda.Desconto,
                    Acrescimo = comanda.Acrescimo,
                    ValorFinal = comanda.ValorFinal,
                    DataAbertura = comanda.DataAbertura
                });
            }
        }

        return comandasPendentes;
    }

    public async Task<VendaComandaDto> ReprocessarPagamentoAsync(int vendaId, PagamentoComandaDto pagamentoDto)
    {
        var venda = await _vendaRepository.GetByIdAsync(vendaId);
        if (venda == null)
            throw new ArgumentException("Venda não encontrada");

        if (venda.Status != StatusVenda.Finalizada)
            throw new InvalidOperationException("Apenas vendas finalizadas podem ser reprocessadas");

        // Cancelar movimentos de caixa anteriores
        // Implementar lógica de cancelamento se necessário

        // Processar novos pagamentos
        return await ProcessarPagamentoComandaAsync(pagamentoDto);
    }

    public async Task<object> GetRelatorioVendasAsync(DateTime dataInicio, DateTime dataFim)
    {
        var vendas = await _vendaRepository.GetByPeriodoAsync(dataInicio, dataFim);
        
        var relatorio = new
        {
            periodo = new { dataInicio, dataFim },
            totalVendas = vendas.Count(),
            valorTotal = vendas.Sum(v => v.ValorTotal),
            valorPago = vendas.Sum(v => v.ValorPago),
            trocoTotal = vendas.Sum(v => v.Troco),
            vendasPorFormaPagamento = vendas
                .GroupBy(v => v.FormaPagamento)
                .Select(g => new { formaPagamento = g.Key.ToString(), quantidade = g.Count(), valor = g.Sum(v => v.ValorTotal) }),
            vendasPorStatus = vendas
                .GroupBy(v => v.Status)
                .Select(g => new { status = g.Key.ToString(), quantidade = g.Count(), valor = g.Sum(v => v.ValorTotal) })
        };

        return relatorio;
    }

    // MÉTODOS PRIVADOS

    private async Task ProcessarBaixaEstoqueAsync(Venda venda)
    {
        if (venda.Itens == null) return;

        foreach (var item in venda.Itens)
        {
            var produto = await _produtoRepository.GetByIdAsync(item.ProdutoId);
            if (produto != null && !produto.ControlaNaoEstoque)
            {
                produto.EstoqueAtual -= item.Quantidade;
                await _produtoRepository.UpdateAsync(produto);

                // Criar movimento de estoque
                var movimento = new MovimentoEstoque
                {
                    ProdutoId = produto.Id,
                    TipoMovimento = TipoMovimentoEstoque.Saida,
                    Quantidade = item.Quantidade,
                    ValorUnitario = item.ValorUnitario,
                    VendaId = venda.Id,
                    Observacoes = $"Venda {venda.NumeroVenda}"
                };

                // Aqui seria adicionado o movimento no repositório específico
                // await _movimentoEstoqueRepository.AddAsync(movimento);
            }
        }
    }

    private async Task<string> GerarNumeroVendaAsync()
    {
        var hoje = DateTime.Now;
        var prefixo = $"VND{hoje:yyyyMMdd}";
        var contador = await _vendaRepository.GetProximoNumeroVendaAsync(prefixo);
        return $"{prefixo}{contador:000}";
    }

    private async Task<VendaDto> MapToDtoAsync(Venda venda)
    {
        return new VendaDto
        {
            Id = venda.Id,
            NumeroVenda = venda.NumeroVenda,
            ClienteId = venda.ClienteId,
            ClienteNome = venda.Cliente?.Nome,
            DataVenda = venda.DataVenda,
            Status = venda.Status,
            SubTotal = venda.SubTotal,
            Desconto = venda.Desconto,
            Acrescimo = venda.Acrescimo,
            ValorTotal = venda.ValorTotal,
            FormaPagamento = venda.FormaPagamento,
            ValorPago = venda.ValorPago,
            Troco = venda.Troco,
            Observacoes = venda.Observacoes,
            VendaBalcao = venda.VendaBalcao,
            ComandaId = venda.ComandaId,
            Itens = venda.Itens?.Select(i => new ItemVendaDto
            {
                Id = i.Id,
                VendaId = i.VendaId,
                ProdutoId = i.ProdutoId,
                ProdutoNome = i.Produto?.Nome ?? "",
                Quantidade = i.Quantidade,
                ValorUnitario = i.ValorUnitario,
                Desconto = i.Desconto,
                ValorTotal = i.ValorTotal,
                Observacoes = i.Observacoes,
                Adicionais = i.Adicionais
            }).ToList() ?? new List<ItemVendaDto>()
        };
    }

    private async Task<VendaComandaDto> MapToVendaComandaDtoAsync(Venda venda, List<FormaPagamentoDto> formasPagamento)
    {
        var comanda = venda.ComandaId.HasValue ? await _comandaRepository.GetByIdAsync(venda.ComandaId.Value) : null;
        
        return new VendaComandaDto
        {
            Id = venda.Id,
            NumeroVenda = venda.NumeroVenda,
            ComandaId = venda.ComandaId ?? 0,
            NumeroComanda = comanda?.NumeroComanda ?? "",
            DataVenda = venda.DataVenda,
            Status = venda.Status,
            SubTotal = venda.SubTotal,
            Desconto = venda.Desconto,
            Acrescimo = venda.Acrescimo,
            ValorTotal = venda.ValorTotal,
            ValorPago = venda.ValorPago,
            Troco = venda.Troco,
            Observacoes = venda.Observacoes,
            OperadorNome = "", // Implementar busca do operador se necessário
            Pagamentos = formasPagamento.Select(fp => new PagamentoVendaDto
            {
                FormaPagamento = fp.FormaPagamento,
                Valor = fp.Valor,
                ValorRecebido = fp.ValorRecebido,
                NumeroDocumento = fp.NumeroDocumento,
                Observacoes = fp.Observacoes,
                DataPagamento = DateTime.UtcNow
            }).ToList(),
            Itens = venda.Itens?.Select(i => new ItemVendaDto
            {
                Id = i.Id,
                VendaId = i.VendaId,
                ProdutoId = i.ProdutoId,
                ProdutoNome = i.Produto?.Nome ?? "",
                Quantidade = i.Quantidade,
                ValorUnitario = i.ValorUnitario,
                Desconto = i.Desconto,
                ValorTotal = i.ValorTotal,
                Observacoes = i.Observacoes,
                Adicionais = i.Adicionais
            }).ToList() ?? new List<ItemVendaDto>()
        };
    }
}