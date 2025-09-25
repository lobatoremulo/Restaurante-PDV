using RestaurantePDV.Domain.Entities;
using RestaurantePDV.Domain.Enums;

namespace RestaurantePDV.Application.Interfaces;

public interface IClienteRestricaoRepository : IBaseRepository<ClienteRestricao>
{
    Task<IEnumerable<ClienteRestricao>> GetByClienteAsync(int clienteId);
    Task<IEnumerable<ClienteRestricao>> GetRestricoesAtivasAsync();
    Task<IEnumerable<ClienteRestricao>> GetByMotivoAsync(MotivoRestricaoCliente motivo);
    Task<IEnumerable<ClienteRestricao>> GetByResponsavelInclusaoAsync(int responsavelId);
    Task<ClienteRestricao?> GetRestricaoAtivaByClienteAsync(int clienteId);
    Task<IEnumerable<ClienteRestricao>> GetComDetalhesAsync();
}