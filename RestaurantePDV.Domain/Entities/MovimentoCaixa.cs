using System.ComponentModel.DataAnnotations;
using RestaurantePDV.Domain.Common;
using RestaurantePDV.Domain.Enums;

namespace RestaurantePDV.Domain.Entities;

public class MovimentoCaixa : BaseEntity
{
    [Required]
    public int CaixaId { get; set; }
    
    [Required]
    public DateTime DataMovimento { get; set; } = DateTime.UtcNow;
    
    [Required]
    public TipoMovimentoCaixa TipoMovimento { get; set; }
    
    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Valor { get; set; }
    
    [Required]
    [StringLength(200)]
    public string Descricao { get; set; } = string.Empty;
    
    [StringLength(500)]
    public string? Observacoes { get; set; }
    
    public FormaPagamento? FormaPagamento { get; set; }
    
    public int? VendaId { get; set; }
    
    public int OperadorId { get; set; }
    
    [StringLength(100)]
    public string? NumeroDocumento { get; set; }
    
    // Relacionamentos
    public virtual Caixa Caixa { get; set; } = null!;
    public virtual Venda? Venda { get; set; }
    public virtual Funcionario Operador { get; set; } = null!;
}