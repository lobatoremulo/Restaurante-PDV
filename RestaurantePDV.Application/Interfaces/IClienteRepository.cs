using RestaurantePDV.Domain.Entities;

namespace RestaurantePDV.Application.Interfaces;

public interface IClienteRepository : IBaseRepository<Cliente>
{
    Task<Cliente?> GetByCpfCnpjAsync(string cpfCnpj);
    Task<IEnumerable<Cliente>> GetByNomeAsync(string nome);
    Task<IEnumerable<Cliente>> GetComLimiteCreditoAsync();
}