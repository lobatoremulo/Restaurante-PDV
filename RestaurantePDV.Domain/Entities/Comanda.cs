using System.ComponentModel.DataAnnotations;
using RestaurantePDV.Domain.Common;
using RestaurantePDV.Domain.Enums;

namespace RestaurantePDV.Domain.Entities;

public class Comanda : BaseEntity
{
    [Required]
    [StringLength(20)]
    public string NumeroComanda { get; set; } = string.Empty;
    
    public int? ClienteId { get; set; }
    
    [Required]
    public DateTime DataAbertura { get; set; } = DateTime.UtcNow;
    
    public DateTime? DataFechamento { get; set; }
    
    [Required]
    public StatusComanda Status { get; set; } = StatusComanda.Aberta;
    
    [StringLength(50)]
    public string? Mesa { get; set; }
    
    [Range(0, double.MaxValue)]
    public decimal ValorTotal { get; set; } = 0;
    
    [Range(0, double.MaxValue)]
    public decimal Desconto { get; set; } = 0;
    
    [Range(0, double.MaxValue)]
    public decimal Acrescimo { get; set; } = 0;
    
    [Range(0, double.MaxValue)]
    public decimal ValorFinal { get; set; } = 0;
    
    [StringLength(500)]
    public string? Observacoes { get; set; }
    
    public int? GarcomId { get; set; }
    
    // Relacionamentos
    public virtual Cliente? Cliente { get; set; }
    public virtual Funcionario? Garcom { get; set; }
    public virtual ICollection<ItemComanda> Itens { get; set; } = new List<ItemComanda>();
    public virtual ICollection<Venda> Vendas { get; set; } = new List<Venda>();
}