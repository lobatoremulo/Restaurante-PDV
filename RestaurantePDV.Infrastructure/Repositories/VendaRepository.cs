using Microsoft.EntityFrameworkCore;
using RestaurantePDV.Application.Interfaces;
using RestaurantePDV.Domain.Entities;
using RestaurantePDV.Domain.Enums;
using RestaurantePDV.Infrastructure.Data;

namespace RestaurantePDV.Infrastructure.Repositories;

public class VendaRepository : BaseRepository<Venda>, IVendaRepository
{
    public VendaRepository(RestauranteContext context) : base(context)
    {
    }

    public async Task<Venda?> GetByNumeroAsync(string numeroVenda)
    {
        return await _dbSet
            .Include(v => v.Cliente)
            .Include(v => v.Itens)
                .ThenInclude(i => i.Produto)
            .Include(v => v.Comanda)
            .FirstOrDefaultAsync(v => v.NumeroVenda == numeroVenda && v.Ativo);
    }

    public async Task<IEnumerable<Venda>> GetByClienteAsync(int clienteId)
    {
        return await _dbSet
            .Include(v => v.Cliente)
            .Where(v => v.ClienteId == clienteId && v.Ativo)
            .OrderByDescending(v => v.DataVenda)
            .ToListAsync();
    }

    public async Task<IEnumerable<Venda>> GetByPeriodoAsync(DateTime dataInicio, DateTime dataFim)
    {
        return await _dbSet
            .Include(v => v.Cliente)
            .Where(v => v.DataVenda >= dataInicio && 
                       v.DataVenda <= dataFim && 
                       v.Ativo)
            .OrderByDescending(v => v.DataVenda)
            .ToListAsync();
    }

    public async Task<IEnumerable<Venda>> GetByStatusAsync(StatusVenda status)
    {
        return await _dbSet
            .Include(v => v.Cliente)
            .Where(v => v.Status == status && v.Ativo)
            .OrderByDescending(v => v.DataVenda)
            .ToListAsync();
    }

    public async Task<Venda?> GetComItensAsync(int id)
    {
        return await _dbSet
            .Include(v => v.Cliente)
            .Include(v => v.Itens)
                .ThenInclude(i => i.Produto)
            .Include(v => v.Comanda)
            .FirstOrDefaultAsync(v => v.Id == id && v.Ativo);
    }

    public async Task<decimal> GetTotalVendasPeriodoAsync(DateTime dataInicio, DateTime dataFim)
    {
        return await _dbSet
            .Where(v => v.DataVenda >= dataInicio && 
                       v.DataVenda <= dataFim && 
                       v.Status == StatusVenda.Finalizada && 
                       v.Ativo)
            .SumAsync(v => v.ValorTotal);
    }
}