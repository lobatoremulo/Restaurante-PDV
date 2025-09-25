using RestaurantePDV.Application.DTOs;
using RestaurantePDV.Application.Interfaces;
using RestaurantePDV.Domain.Entities;
using RestaurantePDV.Domain.Enums;

namespace RestaurantePDV.Application.Services;

public interface IVendaService
{
    Task<VendaDto> CriarVendaAsync(VendaCreateDto vendaDto);
    Task<VendaDto> FinalizarVendaAsync(int vendaId, FormaPagamento formaPagamento, decimal valorPago);
    Task<VendaDto> CancelarVendaAsync(int vendaId);
    Task<VendaDto> GetByIdAsync(int id);
    Task<VendaDto?> GetByNumeroAsync(string numeroVenda);
    Task<IEnumerable<VendaDto>> GetByPeriodoAsync(DateTime dataInicio, DateTime dataFim);
    Task<decimal> GetTotalVendasPeriodoAsync(DateTime dataInicio, DateTime dataFim);
}

public class VendaService : IVendaService
{
    private readonly IVendaRepository _vendaRepository;
    private readonly IProdutoRepository _produtoRepository;
    private readonly IClienteRepository _clienteRepository;

    public VendaService(
        IVendaRepository vendaRepository,
        IProdutoRepository produtoRepository,
        IClienteRepository clienteRepository)
    {
        _vendaRepository = vendaRepository;
        _produtoRepository = produtoRepository;
        _clienteRepository = clienteRepository;
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

    private async Task ProcessarBaixaEstoqueAsync(Venda venda)
    {
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
            }
        }
    }

    private async Task<string> GerarNumeroVendaAsync()
    {
        var hoje = DateTime.Now;
        var prefixo = $"VND{hoje:yyyyMMdd}";
        var contador = 1;

        // Buscar próximo número disponível do dia
        // Implementação simplificada
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
}