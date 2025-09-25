using Microsoft.EntityFrameworkCore;
using RestaurantePDV.Application.Interfaces;
using RestaurantePDV.Domain.Entities;
using RestaurantePDV.Infrastructure.Data;

namespace RestaurantePDV.Infrastructure.Repositories;

public class ClienteRepository : BaseRepository<Cliente>, IClienteRepository
{
    public ClienteRepository(RestauranteContext context) : base(context)
    {
    }

    public async Task<Cliente?> GetByCpfCnpjAsync(string cpfCnpj)
    {
        return await _dbSet
            .FirstOrDefaultAsync(c => c.CpfCnpj == cpfCnpj && c.Ativo);
    }

    public async Task<IEnumerable<Cliente>> GetByNomeAsync(string nome)
    {
        return await _dbSet
            .Where(c => c.Nome.Contains(nome) && c.Ativo)
            .OrderBy(c => c.Nome)
            .ToListAsync();
    }

    public async Task<IEnumerable<Cliente>> GetComLimiteCreditoAsync()
    {
        return await _dbSet
            .Where(c => c.LimiteCredito > 0 && c.Ativo)
            .OrderBy(c => c.Nome)
            .ToListAsync();
    }
}