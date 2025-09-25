using System.ComponentModel.DataAnnotations;
using RestaurantePDV.Domain.Common;

namespace RestaurantePDV.Domain.Entities;

public class Fornecedor : BaseEntity
{
    [Required]
    [StringLength(100)]
    public string RazaoSocial { get; set; } = string.Empty;
    
    [StringLength(100)]
    public string? NomeFantasia { get; set; }
    
    [Required]
    [StringLength(18)]
    public string CnpjCpf { get; set; } = string.Empty;
    
    [StringLength(20)]
    public string? InscricaoEstadual { get; set; }
    
    [StringLength(20)]
    public string? Telefone { get; set; }
    
    [StringLength(100)]
    [EmailAddress]
    public string? Email { get; set; }
    
    [StringLength(200)]
    public string? Endereco { get; set; }
    
    [StringLength(50)]
    public string? Cidade { get; set; }
    
    [StringLength(2)]
    public string? Estado { get; set; }
    
    [StringLength(9)]
    public string? Cep { get; set; }
    
    [StringLength(100)]
    public string? Contato { get; set; }
    
    [StringLength(500)]
    public string? Observacoes { get; set; }
    
    public bool ForneceProduto { get; set; } = true;
    public bool ForneceInsumo { get; set; } = true;
    
    // Relacionamentos
    public virtual ICollection<Insumo> Insumos { get; set; } = new List<Insumo>();
    public virtual ICollection<ContaPagar> ContasPagar { get; set; } = new List<ContaPagar>();
}