using RestaurantePDV.Domain.Entities;
using RestaurantePDV.Domain.Enums;

namespace RestaurantePDV.Application.Interfaces;

public interface IVendaRepository : IBaseRepository<Venda>
{
    Task<Venda?> GetByNumeroAsync(string numeroVenda);
    Task<IEnumerable<Venda>> GetByClienteAsync(int clienteId);
    Task<IEnumerable<Venda>> GetByPeriodoAsync(DateTime dataInicio, DateTime dataFim);
    Task<IEnumerable<Venda>> GetByStatusAsync(StatusVenda status);
    Task<Venda?> GetComItensAsync(int id);
    Task<decimal> GetTotalVendasPeriodoAsync(DateTime dataInicio, DateTime dataFim);
}