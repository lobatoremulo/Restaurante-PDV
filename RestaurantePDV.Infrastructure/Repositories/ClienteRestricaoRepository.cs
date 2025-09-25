using Microsoft.EntityFrameworkCore;
using RestaurantePDV.Application.Interfaces;
using RestaurantePDV.Domain.Entities;
using RestaurantePDV.Domain.Enums;
using RestaurantePDV.Infrastructure.Data;

namespace RestaurantePDV.Infrastructure.Repositories;

public class ClienteRestricaoRepository : BaseRepository<ClienteRestricao>, IClienteRestricaoRepository
{
    public ClienteRestricaoRepository(RestauranteContext context) : base(context)
    {
    }

    public async Task<IEnumerable<ClienteRestricao>> GetByClienteAsync(int clienteId)
    {
        return await _dbSet
            .Include(r => r.Cliente)
            .Include(r => r.ResponsavelInclusao)
            .Include(r => r.ResponsavelRemocao)
            .Where(r => r.ClienteId == clienteId)
            .OrderByDescending(r => r.DataInclusao)
            .ToListAsync();
    }

    public async Task<IEnumerable<ClienteRestricao>> GetRestricoesAtivasAsync()
    {
        return await _dbSet
            .Include(r => r.Cliente)
            .Include(r => r.ResponsavelInclusao)
            .Where(r => r.Ativa && r.Ativo)
            .OrderBy(r => r.Cliente.Nome)
            .ToListAsync();
    }

    public async Task<IEnumerable<ClienteRestricao>> GetByMotivoAsync(MotivoRestricaoCliente motivo)
    {
        return await _dbSet
            .Include(r => r.Cliente)
            .Include(r => r.ResponsavelInclusao)
            .Include(r => r.ResponsavelRemocao)
            .Where(r => r.Motivo == motivo && r.Ativo)
            .OrderByDescending(r => r.DataInclusao)
            .ToListAsync();
    }

    public async Task<IEnumerable<ClienteRestricao>> GetByResponsavelInclusaoAsync(int responsavelId)
    {
        return await _dbSet
            .Include(r => r.Cliente)
            .Include(r => r.ResponsavelInclusao)
            .Include(r => r.ResponsavelRemocao)
            .Where(r => r.ResponsavelInclusaoId == responsavelId && r.Ativo)
            .OrderByDescending(r => r.DataInclusao)
            .ToListAsync();
    }

    public async Task<ClienteRestricao?> GetRestricaoAtivaByClienteAsync(int clienteId)
    {
        return await _dbSet
            .Include(r => r.Cliente)
            .Include(r => r.ResponsavelInclusao)
            .FirstOrDefaultAsync(r => r.ClienteId == clienteId && r.Ativa && r.Ativo);
    }

    public async Task<IEnumerable<ClienteRestricao>> GetComDetalhesAsync()
    {
        return await _dbSet
            .Include(r => r.Cliente)
            .Include(r => r.ResponsavelInclusao)
            .Include(r => r.ResponsavelRemocao)
            .Where(r => r.Ativo)
            .OrderByDescending(r => r.DataInclusao)
            .ToListAsync();
    }

    public override async Task<IEnumerable<ClienteRestricao>> GetAllAsync()
    {
        return await _dbSet
            .Include(r => r.Cliente)
            .Include(r => r.ResponsavelInclusao)
            .Include(r => r.ResponsavelRemocao)
            .OrderByDescending(r => r.DataInclusao)
            .ToListAsync();
    }

    public override async Task<IEnumerable<ClienteRestricao>> GetActiveAsync()
    {
        return await _dbSet
            .Include(r => r.Cliente)
            .Include(r => r.ResponsavelInclusao)
            .Include(r => r.ResponsavelRemocao)
            .Where(r => r.Ativo)
            .OrderByDescending(r => r.DataInclusao)
            .ToListAsync();
    }

    public override async Task<ClienteRestricao?> GetByIdAsync(int id)
    {
        return await _dbSet
            .Include(r => r.Cliente)
            .Include(r => r.ResponsavelInclusao)
            .Include(r => r.ResponsavelRemocao)
            .FirstOrDefaultAsync(r => r.Id == id);
    }
}