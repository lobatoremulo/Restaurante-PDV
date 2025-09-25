using System.ComponentModel.DataAnnotations;

namespace RestaurantePDV.Domain.Common;

public abstract class BaseEntity
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    
    public DateTime? AtualizadoEm { get; set; }
    
    public bool Ativo { get; set; } = true;
}