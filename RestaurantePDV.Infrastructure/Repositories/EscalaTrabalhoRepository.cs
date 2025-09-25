using Microsoft.EntityFrameworkCore;
using RestaurantePDV.Application.Interfaces;
using RestaurantePDV.Domain.Entities;
using RestaurantePDV.Domain.Enums;
using RestaurantePDV.Infrastructure.Data;

namespace RestaurantePDV.Infrastructure.Repositories;

public class EscalaTrabalhoRepository : BaseRepository<EscalaTrabalho>, IEscalaTrabalhoRepository
{
    public EscalaTrabalhoRepository(RestauranteContext context) : base(context)
    {
    }

    public async Task<IEnumerable<EscalaTrabalho>> GetByFuncionarioAsync(int funcionarioId)
    {
        return await _dbSet
            .Include(e => e.Funcionario)
            .Where(e => e.FuncionarioId == funcionarioId && e.Ativo)
            .OrderBy(e => e.DataEscala)
            .ToListAsync();
    }

    public async Task<IEnumerable<EscalaTrabalho>> GetByDataAsync(DateTime data)
    {
        return await _dbSet
            .Include(e => e.Funcionario)
            .Where(e => e.DataEscala.Date == data.Date && e.Ativo)
            .OrderBy(e => e.HoraInicio)
            .ToListAsync();
    }

    public async Task<IEnumerable<EscalaTrabalho>> GetByPeriodoAsync(DateTime dataInicio, DateTime dataFim)
    {
        return await _dbSet
            .Include(e => e.Funcionario)
            .Where(e => e.DataEscala.Date >= dataInicio.Date && 
                       e.DataEscala.Date <= dataFim.Date && e.Ativo)
            .OrderBy(e => e.DataEscala)
            .ThenBy(e => e.HoraInicio)
            .ToListAsync();
    }

    public async Task<IEnumerable<EscalaTrabalho>> GetByTurnoAsync(TurnoTrabalho turno)
    {
        return await _dbSet
            .Include(e => e.Funcionario)
            .Where(e => e.Turno == turno && e.Ativo)
            .OrderBy(e => e.DataEscala)
            .ThenBy(e => e.HoraInicio)
            .ToListAsync();
    }

    public async Task<EscalaTrabalho?> GetByFuncionarioDataAsync(int funcionarioId, DateTime data)
    {
        return await _dbSet
            .Include(e => e.Funcionario)
            .FirstOrDefaultAsync(e => e.FuncionarioId == funcionarioId && 
                                    e.DataEscala.Date == data.Date && e.Ativo);
    }

    public override async Task<IEnumerable<EscalaTrabalho>> GetAllAsync()
    {
        return await _dbSet
            .Include(e => e.Funcionario)
            .OrderBy(e => e.DataEscala)
            .ThenBy(e => e.HoraInicio)
            .ToListAsync();
    }

    public override async Task<IEnumerable<EscalaTrabalho>> GetActiveAsync()
    {
        return await _dbSet
            .Include(e => e.Funcionario)
            .Where(e => e.Ativo)
            .OrderBy(e => e.DataEscala)
            .ThenBy(e => e.HoraInicio)
            .ToListAsync();
    }

    public override async Task<EscalaTrabalho?> GetByIdAsync(int id)
    {
        return await _dbSet
            .Include(e => e.Funcionario)
            .FirstOrDefaultAsync(e => e.Id == id);
    }
}