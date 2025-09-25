using Microsoft.EntityFrameworkCore;
using RestaurantePDV.Application.Interfaces;
using RestaurantePDV.Domain.Entities;
using RestaurantePDV.Domain.Enums;
using RestaurantePDV.Infrastructure.Data;

namespace RestaurantePDV.Infrastructure.Repositories;

public class ProdutoRepository : BaseRepository<Produto>, IProdutoRepository
{
    public ProdutoRepository(RestauranteContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Produto>> GetByTipoAsync(TipoProduto tipo)
    {
        return await _dbSet
            .Where(p => p.Tipo == tipo && p.Ativo)
            .OrderBy(p => p.Nome)
            .ToListAsync();
    }

    public async Task<Produto?> GetByCodigoBarrasAsync(string codigoBarras)
    {
        return await _dbSet
            .FirstOrDefaultAsync(p => p.CodigoBarras == codigoBarras && p.Ativo);
    }

    public async Task<IEnumerable<Produto>> GetByNomeAsync(string nome)
    {
        return await _dbSet
            .Where(p => p.Nome.Contains(nome) && p.Ativo)
            .OrderBy(p => p.Nome)
            .ToListAsync();
    }

    public async Task<IEnumerable<Produto>> GetComEstoqueBaixoAsync()
    {
        return await _dbSet
            .Where(p => !p.ControlaNaoEstoque && 
                       p.EstoqueAtual <= p.EstoqueMinimo && 
                       p.Ativo)
            .OrderBy(p => p.Nome)
            .ToListAsync();
    }

    public async Task<IEnumerable<Produto>> GetDisponivelDeliveryAsync()
    {
        return await _dbSet
            .Where(p => p.DisponivelDelivery && p.Ativo)
            .OrderBy(p => p.Nome)
            .ToListAsync();
    }
}