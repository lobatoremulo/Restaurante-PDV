using System.ComponentModel.DataAnnotations;
using RestaurantePDV.Domain.Common;
using RestaurantePDV.Domain.Enums;

namespace RestaurantePDV.Domain.Entities;

public class Insumo : BaseEntity
{
    [Required]
    [StringLength(100)]
    public string Nome { get; set; } = string.Empty;
    
    [StringLength(500)]
    public string? Descricao { get; set; }
    
    [Required]
    public UnidadeMedida UnidadeMedida { get; set; }
    
    [Range(0, double.MaxValue)]
    public decimal EstoqueAtual { get; set; } = 0;
    
    [Range(0, double.MaxValue)]
    public decimal EstoqueMinimo { get; set; } = 0;
    
    [Range(0, double.MaxValue)]
    public decimal? PrecoCusto { get; set; }
    
    [StringLength(50)]
    public string? CodigoInterno { get; set; }
    
    public DateTime? DataValidade { get; set; }
    
    [StringLength(100)]
    public string? Lote { get; set; }
    
    public int? FornecedorId { get; set; }
    
    // Relacionamentos
    public virtual Fornecedor? Fornecedor { get; set; }
    public virtual ICollection<MovimentoEstoque> MovimentosEstoque { get; set; } = new List<MovimentoEstoque>();
    public virtual ICollection<FichaTecnica> FichasTecnicas { get; set; } = new List<FichaTecnica>();
}