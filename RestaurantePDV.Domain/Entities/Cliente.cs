using System.ComponentModel.DataAnnotations;
using RestaurantePDV.Domain.Common;

namespace RestaurantePDV.Domain.Entities;

public class Cliente : BaseEntity
{
    [Required]
    [StringLength(100)]
    public string Nome { get; set; } = string.Empty;
    
    [StringLength(14)]
    public string? CpfCnpj { get; set; }
    
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
    
    public DateTime? DataNascimento { get; set; }
    
    public decimal LimiteCredito { get; set; } = 0;
    
    [StringLength(500)]
    public string? Observacoes { get; set; }
    
    // Propriedade calculada para verificar se o cliente está restrito
    public bool TemRestricaoAtiva => Restricoes.Any(r => r.Ativa);
    
    // Relacionamentos
    public virtual ICollection<Venda> Vendas { get; set; } = new List<Venda>();
    public virtual ICollection<Comanda> Comandas { get; set; } = new List<Comanda>();
    public virtual ICollection<ClienteRestricao> Restricoes { get; set; } = new List<ClienteRestricao>();
}