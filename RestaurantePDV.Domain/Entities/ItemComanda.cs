using System.ComponentModel.DataAnnotations;
using RestaurantePDV.Domain.Common;

namespace RestaurantePDV.Domain.Entities;

public class ItemComanda : BaseEntity
{
    [Required]
    public int ComandaId { get; set; }
    
    [Required]
    public int ProdutoId { get; set; }
    
    [Required]
    [Range(0.001, double.MaxValue)]
    public decimal Quantidade { get; set; }
    
    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal ValorUnitario { get; set; }
    
    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal ValorTotal { get; set; }
    
    [StringLength(500)]
    public string? Observacoes { get; set; }
    
    [StringLength(500)]
    public string? Adicionais { get; set; }
    
    public bool Preparado { get; set; } = false;
    
    public DateTime? DataPreparo { get; set; }
    
    public bool Entregue { get; set; } = false;
    
    public DateTime? DataEntrega { get; set; }
    
    // Relacionamentos
    public virtual Comanda Comanda { get; set; } = null!;
    public virtual Produto Produto { get; set; } = null!;
}