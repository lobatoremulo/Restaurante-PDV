using System.ComponentModel.DataAnnotations;
using RestaurantePDV.Domain.Common;

namespace RestaurantePDV.Domain.Entities;

public class FichaTecnica : BaseEntity
{
    [Required]
    public int ProdutoId { get; set; }
    
    [Required]
    public int InsumoId { get; set; }
    
    [Required]
    [Range(0.001, double.MaxValue)]
    public decimal Quantidade { get; set; }
    
    [StringLength(200)]
    public string? Observacoes { get; set; }
    
    // Relacionamentos
    public virtual Produto Produto { get; set; } = null!;
    public virtual Insumo Insumo { get; set; } = null!;
}