using RestaurantePDV.Domain.Entities;
using RestaurantePDV.Domain.Enums;

namespace RestaurantePDV.Application.Interfaces;

public interface IEscalaTrabalhoRepository : IBaseRepository<EscalaTrabalho>
{
    Task<IEnumerable<EscalaTrabalho>> GetByFuncionarioAsync(int funcionarioId);
    Task<IEnumerable<EscalaTrabalho>> GetByDataAsync(DateTime data);
    Task<IEnumerable<EscalaTrabalho>> GetByPeriodoAsync(DateTime dataInicio, DateTime dataFim);
    Task<IEnumerable<EscalaTrabalho>> GetByTurnoAsync(TurnoTrabalho turno);
    Task<EscalaTrabalho?> GetByFuncionarioDataAsync(int funcionarioId, DateTime data);
}