using System.ComponentModel.DataAnnotations;
using RestaurantePDV.Domain.Common;
using RestaurantePDV.Domain.Enums;

namespace RestaurantePDV.Domain.Entities;

public class EscalaTrabalho : BaseEntity
{
    [Required]
    public int FuncionarioId { get; set; }
    
    [Required]
    public DateTime DataEscala { get; set; }
    
    [Required]
    public TurnoTrabalho Turno { get; set; }
    
    public TimeSpan HoraInicio { get; set; }
    
    public TimeSpan HoraFim { get; set; }
    
    [StringLength(200)]
    public string? Observacoes { get; set; }
    
    // Relacionamentos
    public virtual Funcionario Funcionario { get; set; } = null!;
}