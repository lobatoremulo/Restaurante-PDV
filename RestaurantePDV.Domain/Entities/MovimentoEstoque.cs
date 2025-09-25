using System.ComponentModel.DataAnnotations;
using RestaurantePDV.Domain.Common;
using RestaurantePDV.Domain.Enums;

namespace RestaurantePDV.Domain.Entities;

public class MovimentoEstoque : BaseEntity
{
    public int? ProdutoId { get; set; }
    
    public int? InsumoId { get; set; }
    
    [Required]
    public TipoMovimentoEstoque TipoMovimento { get; set; }
    
    [Required]
    [Range(0.001, double.MaxValue)]
    public decimal Quantidade { get; set; }
    
    [Range(0, double.MaxValue)]
    public decimal? ValorUnitario { get; set; }
    
    [StringLength(200)]
    public string? Observacoes { get; set; }
    
    [StringLength(100)]
    public string? NumeroDocumento { get; set; }
    
    public int? VendaId { get; set; }
    
    // Relacionamentos
    public virtual Produto? Produto { get; set; }
    public virtual Insumo? Insumo { get; set; }
    public virtual Venda? Venda { get; set; }
}