using RestaurantePDV.Domain.Entities;
using RestaurantePDV.Domain.Enums;

namespace RestaurantePDV.Application.Interfaces;

public interface IProdutoRepository : IBaseRepository<Produto>
{
    Task<IEnumerable<Produto>> GetByTipoAsync(TipoProduto tipo);
    Task<Produto?> GetByCodigoBarrasAsync(string codigoBarras);
    Task<IEnumerable<Produto>> GetByNomeAsync(string nome);
    Task<IEnumerable<Produto>> GetComEstoqueBaixoAsync();
    Task<IEnumerable<Produto>> GetDisponivelDeliveryAsync();
}