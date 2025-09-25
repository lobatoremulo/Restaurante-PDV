using System.ComponentModel.DataAnnotations;
using RestaurantePDV.Domain.Enums;

namespace RestaurantePDV.Application.DTOs;

public class EscalaTrabalhoCreateDto
{
    [Required(ErrorMessage = "ID do funcion�rio � obrigat�rio")]
    public int FuncionarioId { get; set; }
    
    [Required(ErrorMessage = "Data da escala � obrigat�ria")]
    public DateTime DataEscala { get; set; }
    
    [Required(ErrorMessage = "Turno � obrigat�rio")]
    public TurnoTrabalho Turno { get; set; }
    
    [Required(ErrorMessage = "Hora de in�cio � obrigat�ria")]
    public TimeSpan HoraInicio { get; set; }
    
    [Required(ErrorMessage = "Hora de fim � obrigat�ria")]
    public TimeSpan HoraFim { get; set; }
    
    [StringLength(200, ErrorMessage = "Observa��es deve ter no m�ximo 200 caracteres")]
    public string? Observacoes { get; set; }
}

public class EscalaTrabalhoUpdateDto : EscalaTrabalhoCreateDto
{
    [Required]
    public int Id { get; set; }
}

public class EscalaTrabalhoDto
{
    public int Id { get; set; }
    public int FuncionarioId { get; set; }
    public string FuncionarioNome { get; set; } = string.Empty;
    public DateTime DataEscala { get; set; }
    public TurnoTrabalho Turno { get; set; }
    public string TurnoDescricao => Turno.ToString();
    public TimeSpan HoraInicio { get; set; }
    public TimeSpan HoraFim { get; set; }
    public string? Observacoes { get; set; }
    public DateTime CriadoEm { get; set; }
    public DateTime? AtualizadoEm { get; set; }
    public bool Ativo { get; set; }
}

public class EscalaTrabalhoListDto
{
    public int Id { get; set; }
    public int FuncionarioId { get; set; }
    public string FuncionarioNome { get; set; } = string.Empty;
    public DateTime DataEscala { get; set; }
    public TurnoTrabalho Turno { get; set; }
    public string TurnoDescricao => Turno.ToString();
    public TimeSpan HoraInicio { get; set; }
    public TimeSpan HoraFim { get; set; }
}