using Microsoft.EntityFrameworkCore;
using RestaurantePDV.Application.Interfaces;
using RestaurantePDV.Domain.Entities;
using RestaurantePDV.Domain.Enums;
using RestaurantePDV.Infrastructure.Data;

namespace RestaurantePDV.Infrastructure.Repositories;

public class CaixaRepository : BaseRepository<Caixa>, ICaixaRepository
{
    public CaixaRepository(RestauranteContext context) : base(context)
    {
    }

    public override async Task<Caixa?> GetByIdAsync(int id)
    {
        return await _dbSet
            .Include(c => c.OperadorAbertura)
            .Include(c => c.OperadorFechamento)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public override async Task<IEnumerable<Caixa>> GetAllAsync()
    {
        return await _dbSet
            .Include(c => c.OperadorAbertura)
            .Include(c => c.OperadorFechamento)
            .OrderByDescending(c => c.DataAbertura)
            .ToListAsync();
    }

    public async Task<Caixa?> GetCaixaAbertoAsync()
    {
        return await _dbSet
            .Include(c => c.OperadorAbertura)
            .Include(c => c.OperadorFechamento)
            .FirstOrDefaultAsync(c => c.Status == StatusCaixa.Aberto);
    }

    public async Task<IEnumerable<Caixa>> GetByPeriodoAsync(DateTime dataInicio, DateTime dataFim)
    {
        return await _dbSet
            .Include(c => c.OperadorAbertura)
            .Include(c => c.OperadorFechamento)
            .Where(c => c.DataAbertura.Date >= dataInicio.Date && 
                       c.DataAbertura.Date <= dataFim.Date)
            .OrderByDescending(c => c.DataAbertura)
            .ToListAsync();
    }

    public async Task<IEnumerable<Caixa>> GetByOperadorAsync(int operadorId)
    {
        return await _dbSet
            .Include(c => c.OperadorAbertura)
            .Include(c => c.OperadorFechamento)
            .Where(c => c.OperadorAberturaId == operadorId || 
                       c.OperadorFechamentoId == operadorId)
            .OrderByDescending(c => c.DataAbertura)
            .ToListAsync();
    }

    public async Task<Caixa?> GetWithMovimentosAsync(int id)
    {
        return await _dbSet
            .Include(c => c.OperadorAbertura)
            .Include(c => c.OperadorFechamento)
            .Include(c => c.Movimentos)
                .ThenInclude(m => m.Operador)
            .Include(c => c.Movimentos)
                .ThenInclude(m => m.Venda)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<bool> TemCaixaAbertoAsync()
    {
        return await _dbSet.AnyAsync(c => c.Status == StatusCaixa.Aberto);
    }
}

public class MovimentoCaixaRepository : BaseRepository<MovimentoCaixa>, IMovimentoCaixaRepository
{
    public MovimentoCaixaRepository(RestauranteContext context) : base(context)
    {
    }

    public override async Task<MovimentoCaixa?> GetByIdAsync(int id)
    {
        return await _dbSet
            .Include(m => m.Caixa)
            .Include(m => m.Operador)
            .Include(m => m.Venda)
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<IEnumerable<MovimentoCaixa>> GetByCaixaAsync(int caixaId)
    {
        return await _dbSet
            .Include(m => m.Operador)
            .Include(m => m.Venda)
            .Where(m => m.CaixaId == caixaId)
            .OrderBy(m => m.DataMovimento)
            .ToListAsync();
    }

    public async Task<IEnumerable<MovimentoCaixa>> GetByTipoAsync(int caixaId, TipoMovimentoCaixa tipo)
    {
        return await _dbSet
            .Include(m => m.Operador)
            .Include(m => m.Venda)
            .Where(m => m.CaixaId == caixaId && m.TipoMovimento == tipo)
            .OrderBy(m => m.DataMovimento)
            .ToListAsync();
    }

    public async Task<IEnumerable<MovimentoCaixa>> GetByPeriodoAsync(DateTime dataInicio, DateTime dataFim)
    {
        return await _dbSet
            .Include(m => m.Caixa)
            .Include(m => m.Operador)
            .Include(m => m.Venda)
            .Where(m => m.DataMovimento.Date >= dataInicio.Date && 
                       m.DataMovimento.Date <= dataFim.Date)
            .OrderBy(m => m.DataMovimento)
            .ToListAsync();
    }

    public async Task<decimal> GetTotalByTipoAsync(int caixaId, TipoMovimentoCaixa tipo)
    {
        return await _dbSet
            .Where(m => m.CaixaId == caixaId && m.TipoMovimento == tipo)
            .SumAsync(m => m.Valor);
    }

    public async Task<IEnumerable<MovimentoCaixa>> GetVendasByPeriodoAsync(DateTime dataInicio, DateTime dataFim)
    {
        return await _dbSet
            .Include(m => m.Caixa)
            .Include(m => m.Operador)
            .Include(m => m.Venda)
            .Where(m => m.TipoMovimento == TipoMovimentoCaixa.Venda &&
                       m.DataMovimento.Date >= dataInicio.Date && 
                       m.DataMovimento.Date <= dataFim.Date)
            .OrderBy(m => m.DataMovimento)
            .ToListAsync();
    }
}