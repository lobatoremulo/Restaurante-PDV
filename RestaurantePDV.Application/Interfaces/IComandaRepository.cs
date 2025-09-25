using RestaurantePDV.Domain.Entities;
using RestaurantePDV.Domain.Enums;

namespace RestaurantePDV.Application.Interfaces;

public interface IComandaRepository : IBaseRepository<Comanda>
{
    Task<Comanda?> GetByNumeroAsync(string numeroComanda);
    Task<IEnumerable<Comanda>> GetByStatusAsync(StatusComanda status);
    Task<IEnumerable<Comanda>> GetByClienteAsync(int clienteId);
    Task<IEnumerable<Comanda>> GetByPeriodoAsync(DateTime dataInicio, DateTime dataFim);
    Task<Comanda?> GetComItensAsync(int id);
}