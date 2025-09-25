using System.ComponentModel.DataAnnotations;
using RestaurantePDV.Domain.Common;
using RestaurantePDV.Domain.Enums;

namespace RestaurantePDV.Domain.Entities;

public class ClienteRestricao : BaseEntity
{
    [Required]
    public int ClienteId { get; set; }
    
    [Required]
    public MotivoRestricaoCliente Motivo { get; set; }
    
    [Required]
    [StringLength(500)]
    public string Descricao { get; set; } = string.Empty;
    
    [Required]
    public DateTime DataInclusao { get; set; } = DateTime.UtcNow;
    
    public DateTime? DataRemocao { get; set; }
    
    [Required]
    public int ResponsavelInclusaoId { get; set; }
    
    public int? ResponsavelRemocaoId { get; set; }
    
    [StringLength(500)]
    public string? ObservacoesRemocao { get; set; }
    
    public bool Ativa { get; set; } = true;
    
    // Relacionamentos
    public virtual Cliente Cliente { get; set; } = null!;
    public virtual Funcionario ResponsavelInclusao { get; set; } = null!;
    public virtual Funcionario? ResponsavelRemocao { get; set; }
}