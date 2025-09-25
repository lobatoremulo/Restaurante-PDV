using System.ComponentModel.DataAnnotations;
using RestaurantePDV.Domain.Common;
using RestaurantePDV.Domain.Enums;

namespace RestaurantePDV.Domain.Entities;

public class Produto : BaseEntity
{
    [Required]
    [StringLength(100)]
    public string Nome { get; set; } = string.Empty;
    
    [StringLength(500)]
    public string? Descricao { get; set; }
    
    [Required]
    public TipoProduto Tipo { get; set; }
    
    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal PrecoVenda { get; set; }
    
    [Range(0, double.MaxValue)]
    public decimal? PrecoCusto { get; set; }
    
    [StringLength(50)]
    public string? CodigoBarras { get; set; }
    
    [StringLength(20)]
    public string? Unidade { get; set; } = "UN";
    
    public bool ControlaNaoEstoque { get; set; } = false;
    
    public decimal EstoqueMinimo { get; set; } = 0;
    
    public decimal EstoqueAtual { get; set; } = 0;
    
    [StringLength(200)]
    public string? CaminhoFoto { get; set; }
    
    [StringLength(500)]
    public string? Ingredientes { get; set; }
    
    public int? TempoPreparoMinutos { get; set; }
    
    public bool DisponivelDelivery { get; set; } = true;
    
    // Relacionamentos
    public virtual ICollection<ItemVenda> ItensVenda { get; set; } = new List<ItemVenda>();
    public virtual ICollection<ItemComanda> ItensComanda { get; set; } = new List<ItemComanda>();
    public virtual ICollection<FichaTecnica> FichasTecnicas { get; set; } = new List<FichaTecnica>();
}