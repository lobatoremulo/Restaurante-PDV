using System.ComponentModel.DataAnnotations;
using RestaurantePDV.Domain.Common;
using RestaurantePDV.Domain.Enums;

namespace RestaurantePDV.Domain.Entities;

public class PagamentoVenda : BaseEntity
{
    [Required]
    public int VendaId { get; set; }
    
    [Required]
    public FormaPagamento FormaPagamento { get; set; }
    
    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Valor { get; set; }
    
    [Range(0, double.MaxValue)]
    public decimal? ValorRecebido { get; set; }
    
    [StringLength(100)]
    public string? NumeroDocumento { get; set; }
    
    [StringLength(200)]
    public string? Observacoes { get; set; }
    
    [Required]
    public DateTime DataPagamento { get; set; } = DateTime.UtcNow;
    
    // Propriedades calculadas
    public decimal Troco => ValorRecebido.HasValue ? Math.Max(0, ValorRecebido.Value - Valor) : 0;
    
    // Relacionamentos
    public virtual Venda Venda { get; set; } = null!;
}