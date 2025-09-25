using Microsoft.EntityFrameworkCore;
using RestaurantePDV.Application.Interfaces;
using RestaurantePDV.Domain.Entities;
using RestaurantePDV.Domain.Enums;
using RestaurantePDV.Infrastructure.Data;

namespace RestaurantePDV.Infrastructure.Repositories;

public class FuncionarioRepository : BaseRepository<Funcionario>, IFuncionarioRepository
{
    public FuncionarioRepository(RestauranteContext context) : base(context)
    {
    }

    public async Task<Funcionario?> GetByCpfAsync(string cpf)
    {
        return await _dbSet.FirstOrDefaultAsync(f => f.Cpf == cpf);
    }

    public async Task<IEnumerable<Funcionario>> GetByCargoAsync(string cargo)
    {
        return await _dbSet
            .Where(f => f.Cargo.ToLower().Contains(cargo.ToLower()) && f.Ativo)
            .OrderBy(f => f.Nome)
            .ToListAsync();
    }

    public async Task<IEnumerable<Funcionario>> GetBySetorAsync(string setor)
    {
        return await _dbSet
            .Where(f => f.Setor.ToLower().Contains(setor.ToLower()) && f.Ativo)
            .OrderBy(f => f.Nome)
            .ToListAsync();
    }

    public async Task<IEnumerable<Funcionario>> GetByNivelAcessoAsync(NivelAcesso nivelAcesso)
    {
        return await _dbSet
            .Where(f => f.NivelAcesso == nivelAcesso && f.Ativo)
            .OrderBy(f => f.Nome)
            .ToListAsync();
    }

    public async Task<IEnumerable<Funcionario>> GetByStatusAsync(StatusFuncionario status)
    {
        return await _dbSet
            .Where(f => f.Status == status)
            .OrderBy(f => f.Nome)
            .ToListAsync();
    }

    public async Task<Funcionario?> GetComEscalasAsync(int id)
    {
        return await _dbSet
            .Include(f => f.Escalas.Where(e => e.Ativo))
            .FirstOrDefaultAsync(f => f.Id == id);
    }

    public override async Task<IEnumerable<Funcionario>> GetAllAsync()
    {
        return await _dbSet
            .OrderBy(f => f.Nome)
            .ToListAsync();
    }

    public override async Task<IEnumerable<Funcionario>> GetActiveAsync()
    {
        return await _dbSet
            .Where(f => f.Ativo && f.Status == StatusFuncionario.Ativo)
            .OrderBy(f => f.Nome)
            .ToListAsync();
    }
}