using RestaurantePDV.Domain.Entities;
using RestaurantePDV.Domain.Enums;

namespace RestaurantePDV.Application.Interfaces;

public interface ICaixaRepository : IBaseRepository<Caixa>
{
    Task<Caixa?> GetCaixaAbertoAsync();
    Task<IEnumerable<Caixa>> GetByPeriodoAsync(DateTime dataInicio, DateTime dataFim);
    Task<IEnumerable<Caixa>> GetByOperadorAsync(int operadorId);
    Task<Caixa?> GetWithMovimentosAsync(int id);
    Task<bool> TemCaixaAbertoAsync();
}

public interface IMovimentoCaixaRepository : IBaseRepository<MovimentoCaixa>
{
    Task<IEnumerable<MovimentoCaixa>> GetByCaixaAsync(int caixaId);
    Task<IEnumerable<MovimentoCaixa>> GetByTipoAsync(int caixaId, TipoMovimentoCaixa tipo);
    Task<IEnumerable<MovimentoCaixa>> GetByPeriodoAsync(DateTime dataInicio, DateTime dataFim);
    Task<decimal> GetTotalByTipoAsync(int caixaId, TipoMovimentoCaixa tipo);
    Task<IEnumerable<MovimentoCaixa>> GetVendasByPeriodoAsync(DateTime dataInicio, DateTime dataFim);
}