using RestaurantePDV.Domain.Entities;
using RestaurantePDV.Domain.Enums;

namespace RestaurantePDV.Application.Interfaces;

public interface IFuncionarioRepository : IBaseRepository<Funcionario>
{
    Task<Funcionario?> GetByCpfAsync(string cpf);
    Task<IEnumerable<Funcionario>> GetByCargoAsync(string cargo);
    Task<IEnumerable<Funcionario>> GetBySetorAsync(string setor);
    Task<IEnumerable<Funcionario>> GetByNivelAcessoAsync(NivelAcesso nivelAcesso);
    Task<IEnumerable<Funcionario>> GetByStatusAsync(StatusFuncionario status);
    Task<Funcionario?> GetComEscalasAsync(int id);
}