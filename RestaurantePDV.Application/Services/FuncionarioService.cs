using RestaurantePDV.Application.DTOs;
using RestaurantePDV.Application.Interfaces;
using RestaurantePDV.Domain.Entities;
using RestaurantePDV.Domain.Enums;

namespace RestaurantePDV.Application.Services;

public interface IFuncionarioService
{
    Task<FuncionarioDto> CreateAsync(FuncionarioCreateDto funcionarioDto);
    Task<FuncionarioDto> UpdateAsync(FuncionarioUpdateDto funcionarioDto);
    Task<FuncionarioDto> GetByIdAsync(int id);
    Task<FuncionarioDto?> GetByCpfAsync(string cpf);
    Task<IEnumerable<FuncionarioListDto>> GetAllAsync();
    Task<IEnumerable<FuncionarioListDto>> GetActiveAsync();
    Task<IEnumerable<FuncionarioListDto>> GetByCargoAsync(string cargo);
    Task<IEnumerable<FuncionarioListDto>> GetBySetorAsync(string setor);
    Task<IEnumerable<FuncionarioListDto>> GetByNivelAcessoAsync(NivelAcesso nivelAcesso);
    Task<IEnumerable<FuncionarioListDto>> GetByStatusAsync(StatusFuncionario status);
    Task<FuncionarioDto> InativarAsync(int id);
    Task<FuncionarioDto> AtivarAsync(int id);
    Task<FuncionarioDto> DemitirAsync(int id, DateTime dataDemissao);
    Task DeleteAsync(int id);
    Task<bool> CpfExistsAsync(string cpf, int? excludeId = null);
}

public class FuncionarioService : IFuncionarioService
{
    private readonly IFuncionarioRepository _funcionarioRepository;

    public FuncionarioService(IFuncionarioRepository funcionarioRepository)
    {
        _funcionarioRepository = funcionarioRepository;
    }

    public async Task<FuncionarioDto> CreateAsync(FuncionarioCreateDto funcionarioDto)
    {
        // Validar CPF único
        if (await CpfExistsAsync(funcionarioDto.Cpf))
            throw new InvalidOperationException("CPF já cadastrado");

        var funcionario = new Funcionario
        {
            Nome = funcionarioDto.Nome,
            Cpf = funcionarioDto.Cpf,
            Rg = funcionarioDto.Rg,
            Telefone = funcionarioDto.Telefone,
            Email = funcionarioDto.Email,
            Cargo = funcionarioDto.Cargo,
            Setor = funcionarioDto.Setor,
            NivelAcesso = funcionarioDto.NivelAcesso,
            Status = funcionarioDto.Status,
            DataAdmissao = funcionarioDto.DataAdmissao ?? DateTime.UtcNow,
            Salario = funcionarioDto.Salario,
            Endereco = funcionarioDto.Endereco,
            Cidade = funcionarioDto.Cidade,
            Estado = funcionarioDto.Estado,
            Cep = funcionarioDto.Cep,
            DataNascimento = funcionarioDto.DataNascimento,
            Observacoes = funcionarioDto.Observacoes
        };

        var funcionarioCriado = await _funcionarioRepository.AddAsync(funcionario);
        return MapToDto(funcionarioCriado);
    }

    public async Task<FuncionarioDto> UpdateAsync(FuncionarioUpdateDto funcionarioDto)
    {
        var funcionario = await _funcionarioRepository.GetByIdAsync(funcionarioDto.Id);
        if (funcionario == null)
            throw new ArgumentException("Funcionário não encontrado");

        // Validar CPF único (excluindo o próprio funcionário)
        if (await CpfExistsAsync(funcionarioDto.Cpf, funcionarioDto.Id))
            throw new InvalidOperationException("CPF já cadastrado para outro funcionário");

        funcionario.Nome = funcionarioDto.Nome;
        funcionario.Cpf = funcionarioDto.Cpf;
        funcionario.Rg = funcionarioDto.Rg;
        funcionario.Telefone = funcionarioDto.Telefone;
        funcionario.Email = funcionarioDto.Email;
        funcionario.Cargo = funcionarioDto.Cargo;
        funcionario.Setor = funcionarioDto.Setor;
        funcionario.NivelAcesso = funcionarioDto.NivelAcesso;
        funcionario.Status = funcionarioDto.Status;
        funcionario.DataAdmissao = funcionarioDto.DataAdmissao;
        funcionario.DataDemissao = funcionarioDto.DataDemissao;
        funcionario.Salario = funcionarioDto.Salario;
        funcionario.Endereco = funcionarioDto.Endereco;
        funcionario.Cidade = funcionarioDto.Cidade;
        funcionario.Estado = funcionarioDto.Estado;
        funcionario.Cep = funcionarioDto.Cep;
        funcionario.DataNascimento = funcionarioDto.DataNascimento;
        funcionario.Observacoes = funcionarioDto.Observacoes;
        funcionario.AtualizadoEm = DateTime.UtcNow;

        var funcionarioAtualizado = await _funcionarioRepository.UpdateAsync(funcionario);
        return MapToDto(funcionarioAtualizado);
    }

    public async Task<FuncionarioDto> GetByIdAsync(int id)
    {
        var funcionario = await _funcionarioRepository.GetByIdAsync(id);
        if (funcionario == null)
            throw new ArgumentException("Funcionário não encontrado");

        return MapToDto(funcionario);
    }

    public async Task<FuncionarioDto?> GetByCpfAsync(string cpf)
    {
        var funcionario = await _funcionarioRepository.GetByCpfAsync(cpf);
        return funcionario != null ? MapToDto(funcionario) : null;
    }

    public async Task<IEnumerable<FuncionarioListDto>> GetAllAsync()
    {
        var funcionarios = await _funcionarioRepository.GetAllAsync();
        return funcionarios.Select(MapToListDto);
    }

    public async Task<IEnumerable<FuncionarioListDto>> GetActiveAsync()
    {
        var funcionarios = await _funcionarioRepository.GetActiveAsync();
        return funcionarios.Select(MapToListDto);
    }

    public async Task<IEnumerable<FuncionarioListDto>> GetByCargoAsync(string cargo)
    {
        var funcionarios = await _funcionarioRepository.GetByCargoAsync(cargo);
        return funcionarios.Select(MapToListDto);
    }

    public async Task<IEnumerable<FuncionarioListDto>> GetBySetorAsync(string setor)
    {
        var funcionarios = await _funcionarioRepository.GetBySetorAsync(setor);
        return funcionarios.Select(MapToListDto);
    }

    public async Task<IEnumerable<FuncionarioListDto>> GetByNivelAcessoAsync(NivelAcesso nivelAcesso)
    {
        var funcionarios = await _funcionarioRepository.GetByNivelAcessoAsync(nivelAcesso);
        return funcionarios.Select(MapToListDto);
    }

    public async Task<IEnumerable<FuncionarioListDto>> GetByStatusAsync(StatusFuncionario status)
    {
        var funcionarios = await _funcionarioRepository.GetByStatusAsync(status);
        return funcionarios.Select(MapToListDto);
    }

    public async Task<FuncionarioDto> InativarAsync(int id)
    {
        var funcionario = await _funcionarioRepository.GetByIdAsync(id);
        if (funcionario == null)
            throw new ArgumentException("Funcionário não encontrado");

        funcionario.Status = StatusFuncionario.Inativo;
        funcionario.Ativo = false;
        funcionario.AtualizadoEm = DateTime.UtcNow;

        var funcionarioAtualizado = await _funcionarioRepository.UpdateAsync(funcionario);
        return MapToDto(funcionarioAtualizado);
    }

    public async Task<FuncionarioDto> AtivarAsync(int id)
    {
        var funcionario = await _funcionarioRepository.GetByIdAsync(id);
        if (funcionario == null)
            throw new ArgumentException("Funcionário não encontrado");

        funcionario.Status = StatusFuncionario.Ativo;
        funcionario.Ativo = true;
        funcionario.AtualizadoEm = DateTime.UtcNow;

        var funcionarioAtualizado = await _funcionarioRepository.UpdateAsync(funcionario);
        return MapToDto(funcionarioAtualizado);
    }

    public async Task<FuncionarioDto> DemitirAsync(int id, DateTime dataDemissao)
    {
        var funcionario = await _funcionarioRepository.GetByIdAsync(id);
        if (funcionario == null)
            throw new ArgumentException("Funcionário não encontrado");

        funcionario.Status = StatusFuncionario.Inativo;
        funcionario.DataDemissao = dataDemissao;
        funcionario.Ativo = false;
        funcionario.AtualizadoEm = DateTime.UtcNow;

        var funcionarioAtualizado = await _funcionarioRepository.UpdateAsync(funcionario);
        return MapToDto(funcionarioAtualizado);
    }

    public async Task DeleteAsync(int id)
    {
        var funcionario = await _funcionarioRepository.GetByIdAsync(id);
        if (funcionario == null)
            throw new ArgumentException("Funcionário não encontrado");

        await _funcionarioRepository.DeleteAsync(id);
    }

    public async Task<bool> CpfExistsAsync(string cpf, int? excludeId = null)
    {
        var funcionario = await _funcionarioRepository.GetByCpfAsync(cpf);
        return funcionario != null && (excludeId == null || funcionario.Id != excludeId);
    }

    private FuncionarioDto MapToDto(Funcionario funcionario)
    {
        return new FuncionarioDto
        {
            Id = funcionario.Id,
            Nome = funcionario.Nome,
            Cpf = funcionario.Cpf,
            Rg = funcionario.Rg,
            Telefone = funcionario.Telefone,
            Email = funcionario.Email,
            Cargo = funcionario.Cargo,
            Setor = funcionario.Setor,
            NivelAcesso = funcionario.NivelAcesso,
            Status = funcionario.Status,
            DataAdmissao = funcionario.DataAdmissao,
            DataDemissao = funcionario.DataDemissao,
            Salario = funcionario.Salario,
            Endereco = funcionario.Endereco,
            Cidade = funcionario.Cidade,
            Estado = funcionario.Estado,
            Cep = funcionario.Cep,
            DataNascimento = funcionario.DataNascimento,
            Observacoes = funcionario.Observacoes,
            CriadoEm = funcionario.CriadoEm,
            AtualizadoEm = funcionario.AtualizadoEm,
            Ativo = funcionario.Ativo
        };
    }

    private FuncionarioListDto MapToListDto(Funcionario funcionario)
    {
        return new FuncionarioListDto
        {
            Id = funcionario.Id,
            Nome = funcionario.Nome,
            Cpf = funcionario.Cpf,
            Cargo = funcionario.Cargo,
            Setor = funcionario.Setor,
            NivelAcesso = funcionario.NivelAcesso,
            Status = funcionario.Status,
            DataAdmissao = funcionario.DataAdmissao,
            Ativo = funcionario.Ativo
        };
    }
}