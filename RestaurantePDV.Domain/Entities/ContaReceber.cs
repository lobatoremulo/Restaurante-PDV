using System.ComponentModel.DataAnnotations;
using RestaurantePDV.Domain.Common;
using RestaurantePDV.Domain.Enums;

namespace RestaurantePDV.Domain.Entities;

public class ContaReceber : BaseEntity
{
    [Required]
    [StringLength(100)]
    public string Descricao { get; set; } = string.Empty;
    
    public int? ClienteId { get; set; }
    
    public int? VendaId { get; set; }
    
    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal ValorOriginal { get; set; }
    
    [Range(0, double.MaxValue)]
    public decimal ValorRecebido { get; set; } = 0;
    
    [Range(0, double.MaxValue)]
    public decimal ValorDesconto { get; set; } = 0;
    
    [Range(0, double.MaxValue)]
    public decimal ValorJuros { get; set; } = 0;
    
    [Required]
    public DateTime DataVencimento { get; set; }
    
    public DateTime? DataRecebimento { get; set; }
    
    [Required]
    public StatusConta Status { get; set; } = StatusConta.Pendente;
    
    [StringLength(100)]
    public string? NumeroDocumento { get; set; }
    
    [StringLength(500)]
    public string? Observacoes { get; set; }
    
    public FormaPagamento? FormaPagamento { get; set; }
    
    // Relacionamentos
    public virtual Cliente? Cliente { get; set; }
    public virtual Venda? Venda { get; set; }
}