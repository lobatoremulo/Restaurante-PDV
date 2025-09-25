using Microsoft.EntityFrameworkCore;
using RestaurantePDV.Application.Interfaces;
using RestaurantePDV.Domain.Common;
using RestaurantePDV.Infrastructure.Data;

namespace RestaurantePDV.Infrastructure.Repositories;

public class BaseRepository<T> : IBaseRepository<T> where T : BaseEntity
{
    protected readonly RestauranteContext _context;
    protected readonly DbSet<T> _dbSet;

    public BaseRepository(RestauranteContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public virtual async Task<T?> GetByIdAsync(int id)
    {
        return await _dbSet.FirstOrDefaultAsync(e => e.Id == id);
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public virtual async Task<IEnumerable<T>> GetActiveAsync()
    {
        return await _dbSet.Where(e => e.Ativo).ToListAsync();
    }

    public virtual async Task<T> AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public virtual async Task<T> UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public virtual async Task DeleteAsync(int id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null)
        {
            entity.Ativo = false;
            entity.AtualizadoEm = DateTime.UtcNow;
            await UpdateAsync(entity);
        }
    }

    public virtual async Task<bool> ExistsAsync(int id)
    {
        return await _dbSet.AnyAsync(e => e.Id == id);
    }
}