using System.ComponentModel.DataAnnotations;
using RestaurantePDV.Domain.Common;

namespace RestaurantePDV.Domain.Entities;

public class ItemVenda : BaseEntity
{
    [Required]
    public int VendaId { get; set; }
    
    [Required]
    public int ProdutoId { get; set; }
    
    [Required]
    [Range(0.001, double.MaxValue)]
    public decimal Quantidade { get; set; }
    
    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal ValorUnitario { get; set; }
    
    [Range(0, double.MaxValue)]
    public decimal Desconto { get; set; } = 0;
    
    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal ValorTotal { get; set; }
    
    [StringLength(500)]
    public string? Observacoes { get; set; }
    
    [StringLength(500)]
    public string? Adicionais { get; set; }
    
    // Relacionamentos
    public virtual Venda Venda { get; set; } = null!;
    public virtual Produto Produto { get; set; } = null!;
}