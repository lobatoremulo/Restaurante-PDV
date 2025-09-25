using RestaurantePDV.Application.DTOs;
using RestaurantePDV.Application.Interfaces;
using RestaurantePDV.Domain.Entities;
using RestaurantePDV.Domain.Enums;

namespace RestaurantePDV.Application.Services;

public interface IComandaService
{
    Task<ComandaDto> AbrirComandaAsync(ComandaCreateDto comandaDto);
    Task<ComandaDto> FecharComandaAsync(int comandaId);
    Task<ComandaDto> CancelarComandaAsync(int comandaId);
    Task<ComandaDto> AdicionarItemAsync(int comandaId, ItemComandaCreateDto itemDto);
    Task<ComandaDto> RemoverItemAsync(int comandaId, int itemId);
    Task<ComandaDto> MarcarItemPreparadoAsync(int itemId);
    Task<ComandaDto> MarcarItemEntregueAsync(int itemId);
    Task<ComandaDto> GetByIdAsync(int id);
    Task<ComandaDto?> GetByNumeroAsync(string numeroComanda);
    Task<IEnumerable<ComandaDto>> GetAtivasAsync();
    Task<IEnumerable<ComandaDto>> GetByPeriodoAsync(DateTime dataInicio, DateTime dataFim);
}

public class ComandaService : IComandaService
{
    private readonly IComandaRepository _comandaRepository;
    private readonly IProdutoRepository _produtoRepository;

    public ComandaService(IComandaRepository comandaRepository, IProdutoRepository produtoRepository)
    {
        _comandaRepository = comandaRepository;
        _produtoRepository = produtoRepository;
    }

    public async Task<ComandaDto> AbrirComandaAsync(ComandaCreateDto comandaDto)
    {
        var numeroComanda = await GerarNumeroComandaAsync();
        
        var comanda = new Comanda
        {
            NumeroComanda = numeroComanda,
            ClienteId = comandaDto.ClienteId,
            Mesa = comandaDto.Mesa,
            Observacoes = comandaDto.Observacoes,
            GarcomId = comandaDto.GarcomId,
            Status = StatusComanda.Aberta,
            DataAbertura = DateTime.UtcNow
        };

        var comandaCriada = await _comandaRepository.AddAsync(comanda);
        return MapToDto(comandaCriada);
    }

    public async Task<ComandaDto> FecharComandaAsync(int comandaId)
    {
        var comanda = await _comandaRepository.GetComItensAsync(comandaId);
        if (comanda == null)
            throw new ArgumentException("Comanda não encontrada");

        if (comanda.Status != StatusComanda.Aberta)
            throw new InvalidOperationException("Comanda não está aberta");

        comanda.Status = StatusComanda.Fechada;
        comanda.DataFechamento = DateTime.UtcNow;

        // Calcular valor final
        comanda.ValorTotal = comanda.Itens.Sum(i => i.ValorTotal);
        comanda.ValorFinal = comanda.ValorTotal - comanda.Desconto + comanda.Acrescimo;

        var comandaAtualizada = await _comandaRepository.UpdateAsync(comanda);
        return MapToDto(comandaAtualizada);
    }

    public async Task<ComandaDto> CancelarComandaAsync(int comandaId)
    {
        var comanda = await _comandaRepository.GetByIdAsync(comandaId);
        if (comanda == null)
            throw new ArgumentException("Comanda não encontrada");

        if (comanda.Status == StatusComanda.Fechada)
            throw new InvalidOperationException("Comanda já está fechada");

        comanda.Status = StatusComanda.Cancelada;
        comanda.DataFechamento = DateTime.UtcNow;

        var comandaAtualizada = await _comandaRepository.UpdateAsync(comanda);
        return MapToDto(comandaAtualizada);
    }

    public async Task<ComandaDto> AdicionarItemAsync(int comandaId, ItemComandaCreateDto itemDto)
    {
        var comanda = await _comandaRepository.GetComItensAsync(comandaId);
        if (comanda == null)
            throw new ArgumentException("Comanda não encontrada");

        if (comanda.Status != StatusComanda.Aberta)
            throw new InvalidOperationException("Comanda não está aberta");

        var produto = await _produtoRepository.GetByIdAsync(itemDto.ProdutoId);
        if (produto == null)
            throw new ArgumentException("Produto não encontrado");

        var item = new ItemComanda
        {
            ComandaId = comandaId,
            ProdutoId = itemDto.ProdutoId,
            Quantidade = itemDto.Quantidade,
            ValorUnitario = produto.PrecoVenda,
            ValorTotal = produto.PrecoVenda * itemDto.Quantidade,
            Observacoes = itemDto.Observacoes,
            Adicionais = itemDto.Adicionais
        };

        comanda.Itens.Add(item);
        var comandaAtualizada = await _comandaRepository.UpdateAsync(comanda);
        return MapToDto(comandaAtualizada);
    }

    public async Task<ComandaDto> RemoverItemAsync(int comandaId, int itemId)
    {
        var comanda = await _comandaRepository.GetComItensAsync(comandaId);
        if (comanda == null)
            throw new ArgumentException("Comanda não encontrada");

        if (comanda.Status != StatusComanda.Aberta)
            throw new InvalidOperationException("Comanda não está aberta");

        var item = comanda.Itens.FirstOrDefault(i => i.Id == itemId);
        if (item == null)
            throw new ArgumentException("Item não encontrado na comanda");

        comanda.Itens.Remove(item);
        var comandaAtualizada = await _comandaRepository.UpdateAsync(comanda);
        return MapToDto(comandaAtualizada);
    }

    public async Task<ComandaDto> MarcarItemPreparadoAsync(int itemId)
    {
        // Implementação simplificada - seria necessário um repositório específico para ItemComanda
        throw new NotImplementedException("Implementar busca de item por ID");
    }

    public async Task<ComandaDto> MarcarItemEntregueAsync(int itemId)
    {
        // Implementação simplificada - seria necessário um repositório específico para ItemComanda
        throw new NotImplementedException("Implementar busca de item por ID");
    }

    public async Task<ComandaDto> GetByIdAsync(int id)
    {
        var comanda = await _comandaRepository.GetComItensAsync(id);
        if (comanda == null)
            throw new ArgumentException("Comanda não encontrada");

        return MapToDto(comanda);
    }

    public async Task<ComandaDto?> GetByNumeroAsync(string numeroComanda)
    {
        var comanda = await _comandaRepository.GetByNumeroAsync(numeroComanda);
        return comanda != null ? MapToDto(comanda) : null;
    }

    public async Task<IEnumerable<ComandaDto>> GetAtivasAsync()
    {
        var comandas = await _comandaRepository.GetByStatusAsync(StatusComanda.Aberta);
        return comandas.Select(MapToDto);
    }

    public async Task<IEnumerable<ComandaDto>> GetByPeriodoAsync(DateTime dataInicio, DateTime dataFim)
    {
        var comandas = await _comandaRepository.GetByPeriodoAsync(dataInicio, dataFim);
        return comandas.Select(MapToDto);
    }

    private async Task<string> GerarNumeroComandaAsync()
    {
        var hoje = DateTime.Now;
        var prefixo = $"CMD{hoje:yyyyMMdd}";
        var contador = 1;

        // Buscar próximo número disponível do dia
        // Implementação simplificada
        return $"{prefixo}{contador:000}";
    }

    private ComandaDto MapToDto(Comanda comanda)
    {
        return new ComandaDto
        {
            Id = comanda.Id,
            NumeroComanda = comanda.NumeroComanda,
            ClienteId = comanda.ClienteId,
            ClienteNome = comanda.Cliente?.Nome,
            DataAbertura = comanda.DataAbertura,
            DataFechamento = comanda.DataFechamento,
            Status = comanda.Status,
            Mesa = comanda.Mesa,
            ValorTotal = comanda.ValorTotal,
            Desconto = comanda.Desconto,
            Acrescimo = comanda.Acrescimo,
            ValorFinal = comanda.ValorFinal,
            Observacoes = comanda.Observacoes,
            GarcomId = comanda.GarcomId,
            CriadoEm = comanda.CriadoEm,
            Itens = comanda.Itens?.Select(i => new ItemComandaDto
            {
                Id = i.Id,
                ComandaId = i.ComandaId,
                ProdutoId = i.ProdutoId,
                ProdutoNome = i.Produto?.Nome ?? "",
                Quantidade = i.Quantidade,
                ValorUnitario = i.ValorUnitario,
                ValorTotal = i.ValorTotal,
                Observacoes = i.Observacoes,
                Adicionais = i.Adicionais,
                Preparado = i.Preparado,
                DataPreparo = i.DataPreparo,
                Entregue = i.Entregue,
                DataEntrega = i.DataEntrega
            }).ToList() ?? new List<ItemComandaDto>()
        };
    }
}