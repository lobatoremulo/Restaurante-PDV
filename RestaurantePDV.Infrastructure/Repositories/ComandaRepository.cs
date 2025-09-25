using Microsoft.EntityFrameworkCore;
using RestaurantePDV.Application.Interfaces;
using RestaurantePDV.Domain.Entities;
using RestaurantePDV.Domain.Enums;
using RestaurantePDV.Infrastructure.Data;

namespace RestaurantePDV.Infrastructure.Repositories;

public class ComandaRepository : BaseRepository<Comanda>, IComandaRepository
{
    public ComandaRepository(RestauranteContext context) : base(context)
    {
    }

    public async Task<Comanda?> GetByNumeroAsync(string numeroComanda)
    {
        return await _dbSet
            .Include(c => c.Cliente)
            .Include(c => c.Itens)
                .ThenInclude(i => i.Produto)
            .FirstOrDefaultAsync(c => c.NumeroComanda == numeroComanda && c.Ativo);
    }

    public async Task<IEnumerable<Comanda>> GetByStatusAsync(StatusComanda status)
    {
        return await _dbSet
            .Include(c => c.Cliente)
            .Where(c => c.Status == status && c.Ativo)
            .OrderByDescending(c => c.DataAbertura)
            .ToListAsync();
    }

    public async Task<IEnumerable<Comanda>> GetByClienteAsync(int clienteId)
    {
        return await _dbSet
            .Include(c => c.Cliente)
            .Where(c => c.ClienteId == clienteId && c.Ativo)
            .OrderByDescending(c => c.DataAbertura)
            .ToListAsync();
    }

    public async Task<IEnumerable<Comanda>> GetByPeriodoAsync(DateTime dataInicio, DateTime dataFim)
    {
        return await _dbSet
            .Include(c => c.Cliente)
            .Where(c => c.DataAbertura >= dataInicio && 
                       c.DataAbertura <= dataFim && 
                       c.Ativo)
            .OrderByDescending(c => c.DataAbertura)
            .ToListAsync();
    }

    public async Task<Comanda?> GetComItensAsync(int id)
    {
        return await _dbSet
            .Include(c => c.Cliente)
            .Include(c => c.Itens)
                .ThenInclude(i => i.Produto)
            .FirstOrDefaultAsync(c => c.Id == id && c.Ativo);
    }
}