using System.ComponentModel.DataAnnotations;
using RestaurantePDV.Domain.Common;
using RestaurantePDV.Domain.Enums;

namespace RestaurantePDV.Domain.Entities;

public class Venda : BaseEntity
{
    [Required]
    [StringLength(20)]
    public string NumeroVenda { get; set; } = string.Empty;
    
    public int? ClienteId { get; set; }
    
    [Required]
    public DateTime DataVenda { get; set; } = DateTime.UtcNow;
    
    [Required]
    public StatusVenda Status { get; set; } = StatusVenda.Aberta;
    
    [Required]
    [Range(0, double.MaxValue)]
    public decimal SubTotal { get; set; } = 0;
    
    [Range(0, double.MaxValue)]
    public decimal Desconto { get; set; } = 0;
    
    [Range(0, double.MaxValue)]
    public decimal Acrescimo { get; set; } = 0;
    
    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal ValorTotal { get; set; } = 0;
    
    public FormaPagamento FormaPagamento { get; set; }
    
    [Range(0, double.MaxValue)]
    public decimal ValorPago { get; set; } = 0;
    
    [Range(0, double.MaxValue)]
    public decimal Troco { get; set; } = 0;
    
    [StringLength(500)]
    public string? Observacoes { get; set; }
    
    public bool VendaBalcao { get; set; } = true;
    
    public int? ComandaId { get; set; }
    
    // Relacionamentos
    public virtual Cliente? Cliente { get; set; }
    public virtual Comanda? Comanda { get; set; }
    public virtual ICollection<ItemVenda> Itens { get; set; } = new List<ItemVenda>();
    public virtual ICollection<MovimentoEstoque> MovimentosEstoque { get; set; } = new List<MovimentoEstoque>();
}