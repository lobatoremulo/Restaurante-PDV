using System.ComponentModel.DataAnnotations;
using RestaurantePDV.Domain.Common;
using RestaurantePDV.Domain.Enums;

namespace RestaurantePDV.Domain.Entities;

public class Funcionario : BaseEntity
{
    [Required]
    [StringLength(100)]
    public string Nome { get; set; } = string.Empty;
    
    [Required]
    [StringLength(14)]
    public string Cpf { get; set; } = string.Empty;
    
    [StringLength(15)]
    public string? Rg { get; set; }
    
    [StringLength(20)]
    public string? Telefone { get; set; }
    
    [StringLength(100)]
    [EmailAddress]
    public string? Email { get; set; }
    
    [Required]
    [StringLength(100)]
    public string Cargo { get; set; } = string.Empty;
    
    [Required]
    [StringLength(100)]
    public string Setor { get; set; } = string.Empty;
    
    [Required]
    public NivelAcesso NivelAcesso { get; set; }
    
    [Required]
    public StatusFuncionario Status { get; set; } = StatusFuncionario.Ativo;
    
    public DateTime? DataAdmissao { get; set; }
    
    public DateTime? DataDemissao { get; set; }
    
    [Range(0, double.MaxValue)]
    public decimal? Salario { get; set; }
    
    [StringLength(200)]
    public string? Endereco { get; set; }
    
    [StringLength(50)]
    public string? Cidade { get; set; }
    
    [StringLength(2)]
    public string? Estado { get; set; }
    
    [StringLength(9)]
    public string? Cep { get; set; }
    
    public DateTime? DataNascimento { get; set; }
    
    [StringLength(500)]
    public string? Observacoes { get; set; }
    
    // Relacionamentos
    public virtual ICollection<EscalaTrabalho> Escalas { get; set; } = new List<EscalaTrabalho>();
    public virtual ICollection<Comanda> Comandas { get; set; } = new List<Comanda>();
}