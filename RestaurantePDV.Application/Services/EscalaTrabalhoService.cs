using RestaurantePDV.Application.DTOs;
using RestaurantePDV.Application.Interfaces;
using RestaurantePDV.Domain.Entities;
using RestaurantePDV.Domain.Enums;

namespace RestaurantePDV.Application.Services;

public interface IEscalaTrabalhoService
{
    Task<EscalaTrabalhoDto> CreateAsync(EscalaTrabalhoCreateDto escalaDto);
    Task<EscalaTrabalhoDto> UpdateAsync(EscalaTrabalhoUpdateDto escalaDto);
    Task<EscalaTrabalhoDto> GetByIdAsync(int id);
    Task<IEnumerable<EscalaTrabalhoListDto>> GetAllAsync();
    Task<IEnumerable<EscalaTrabalhoListDto>> GetByFuncionarioAsync(int funcionarioId);
    Task<IEnumerable<EscalaTrabalhoListDto>> GetByDataAsync(DateTime data);
    Task<IEnumerable<EscalaTrabalhoListDto>> GetByPeriodoAsync(DateTime dataInicio, DateTime dataFim);
    Task<IEnumerable<EscalaTrabalhoListDto>> GetByTurnoAsync(TurnoTrabalho turno);
    Task<EscalaTrabalhoDto?> GetByFuncionarioDataAsync(int funcionarioId, DateTime data);
    Task DeleteAsync(int id);
    Task<bool> FuncionarioTemEscalaAsync(int funcionarioId, DateTime data);
}

public class EscalaTrabalhoService : IEscalaTrabalhoService
{
    private readonly IEscalaTrabalhoRepository _escalaRepository;
    private readonly IFuncionarioRepository _funcionarioRepository;

    public EscalaTrabalhoService(IEscalaTrabalhoRepository escalaRepository, IFuncionarioRepository funcionarioRepository)
    {
        _escalaRepository = escalaRepository;
        _funcionarioRepository = funcionarioRepository;
    }

    public async Task<EscalaTrabalhoDto> CreateAsync(EscalaTrabalhoCreateDto escalaDto)
    {
        // Validar se funcionário existe
        var funcionario = await _funcionarioRepository.GetByIdAsync(escalaDto.FuncionarioId);
        if (funcionario == null)
            throw new ArgumentException("Funcionário não encontrado");

        // Validar se funcionário já tem escala para a data
        if (await FuncionarioTemEscalaAsync(escalaDto.FuncionarioId, escalaDto.DataEscala))
            throw new InvalidOperationException("Funcionário já possui escala para esta data");

        // Validar horários
        if (escalaDto.HoraFim <= escalaDto.HoraInicio)
            throw new ArgumentException("Hora de fim deve ser posterior à hora de início");

        var escala = new EscalaTrabalho
        {
            FuncionarioId = escalaDto.FuncionarioId,
            DataEscala = escalaDto.DataEscala,
            Turno = escalaDto.Turno,
            HoraInicio = escalaDto.HoraInicio,
            HoraFim = escalaDto.HoraFim,
            Observacoes = escalaDto.Observacoes
        };

        var escalaCriada = await _escalaRepository.AddAsync(escala);
        return await MapToDtoAsync(escalaCriada);
    }

    public async Task<EscalaTrabalhoDto> UpdateAsync(EscalaTrabalhoUpdateDto escalaDto)
    {
        var escala = await _escalaRepository.GetByIdAsync(escalaDto.Id);
        if (escala == null)
            throw new ArgumentException("Escala não encontrada");

        // Validar se funcionário existe
        var funcionario = await _funcionarioRepository.GetByIdAsync(escalaDto.FuncionarioId);
        if (funcionario == null)
            throw new ArgumentException("Funcionário não encontrado");

        // Validar se funcionário já tem escala para a data (excluindo a escala atual)
        var escalaExistente = await _escalaRepository.GetByFuncionarioDataAsync(escalaDto.FuncionarioId, escalaDto.DataEscala);
        if (escalaExistente != null && escalaExistente.Id != escalaDto.Id)
            throw new InvalidOperationException("Funcionário já possui escala para esta data");

        // Validar horários
        if (escalaDto.HoraFim <= escalaDto.HoraInicio)
            throw new ArgumentException("Hora de fim deve ser posterior à hora de início");

        escala.FuncionarioId = escalaDto.FuncionarioId;
        escala.DataEscala = escalaDto.DataEscala;
        escala.Turno = escalaDto.Turno;
        escala.HoraInicio = escalaDto.HoraInicio;
        escala.HoraFim = escalaDto.HoraFim;
        escala.Observacoes = escalaDto.Observacoes;
        escala.AtualizadoEm = DateTime.UtcNow;

        var escalaAtualizada = await _escalaRepository.UpdateAsync(escala);
        return await MapToDtoAsync(escalaAtualizada);
    }

    public async Task<EscalaTrabalhoDto> GetByIdAsync(int id)
    {
        var escala = await _escalaRepository.GetByIdAsync(id);
        if (escala == null)
            throw new ArgumentException("Escala não encontrada");

        return await MapToDtoAsync(escala);
    }

    public async Task<IEnumerable<EscalaTrabalhoListDto>> GetAllAsync()
    {
        var escalas = await _escalaRepository.GetAllAsync();
        return await MapToListDtoAsync(escalas);
    }

    public async Task<IEnumerable<EscalaTrabalhoListDto>> GetByFuncionarioAsync(int funcionarioId)
    {
        var escalas = await _escalaRepository.GetByFuncionarioAsync(funcionarioId);
        return await MapToListDtoAsync(escalas);
    }

    public async Task<IEnumerable<EscalaTrabalhoListDto>> GetByDataAsync(DateTime data)
    {
        var escalas = await _escalaRepository.GetByDataAsync(data);
        return await MapToListDtoAsync(escalas);
    }

    public async Task<IEnumerable<EscalaTrabalhoListDto>> GetByPeriodoAsync(DateTime dataInicio, DateTime dataFim)
    {
        var escalas = await _escalaRepository.GetByPeriodoAsync(dataInicio, dataFim);
        return await MapToListDtoAsync(escalas);
    }

    public async Task<IEnumerable<EscalaTrabalhoListDto>> GetByTurnoAsync(TurnoTrabalho turno)
    {
        var escalas = await _escalaRepository.GetByTurnoAsync(turno);
        return await MapToListDtoAsync(escalas);
    }

    public async Task<EscalaTrabalhoDto?> GetByFuncionarioDataAsync(int funcionarioId, DateTime data)
    {
        var escala = await _escalaRepository.GetByFuncionarioDataAsync(funcionarioId, data);
        return escala != null ? await MapToDtoAsync(escala) : null;
    }

    public async Task DeleteAsync(int id)
    {
        var escala = await _escalaRepository.GetByIdAsync(id);
        if (escala == null)
            throw new ArgumentException("Escala não encontrada");

        await _escalaRepository.DeleteAsync(id);
    }

    public async Task<bool> FuncionarioTemEscalaAsync(int funcionarioId, DateTime data)
    {
        var escala = await _escalaRepository.GetByFuncionarioDataAsync(funcionarioId, data);
        return escala != null;
    }

    private async Task<EscalaTrabalhoDto> MapToDtoAsync(EscalaTrabalho escala)
    {
        var funcionario = await _funcionarioRepository.GetByIdAsync(escala.FuncionarioId);
        
        return new EscalaTrabalhoDto
        {
            Id = escala.Id,
            FuncionarioId = escala.FuncionarioId,
            FuncionarioNome = funcionario?.Nome ?? "",
            DataEscala = escala.DataEscala,
            Turno = escala.Turno,
            HoraInicio = escala.HoraInicio,
            HoraFim = escala.HoraFim,
            Observacoes = escala.Observacoes,
            CriadoEm = escala.CriadoEm,
            AtualizadoEm = escala.AtualizadoEm,
            Ativo = escala.Ativo
        };
    }

    private async Task<IEnumerable<EscalaTrabalhoListDto>> MapToListDtoAsync(IEnumerable<EscalaTrabalho> escalas)
    {
        var result = new List<EscalaTrabalhoListDto>();
        
        foreach (var escala in escalas)
        {
            var funcionario = await _funcionarioRepository.GetByIdAsync(escala.FuncionarioId);
            
            result.Add(new EscalaTrabalhoListDto
            {
                Id = escala.Id,
                FuncionarioId = escala.FuncionarioId,
                FuncionarioNome = funcionario?.Nome ?? "",
                DataEscala = escala.DataEscala,
                Turno = escala.Turno,
                HoraInicio = escala.HoraInicio,
                HoraFim = escala.HoraFim
            });
        }
        
        return result;
    }
}