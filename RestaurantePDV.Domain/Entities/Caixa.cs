using System.ComponentModel.DataAnnotations;
using RestaurantePDV.Domain.Common;
using RestaurantePDV.Domain.Enums;

namespace RestaurantePDV.Domain.Entities;

public class Caixa : BaseEntity
{
    [Required]
    public DateTime DataAbertura { get; set; } = DateTime.UtcNow;
    
    public DateTime? DataFechamento { get; set; }
    
    [Required]
    public StatusCaixa Status { get; set; } = StatusCaixa.Aberto;
    
    [Required]
    [Range(0, double.MaxValue)]
    public decimal ValorAbertura { get; set; } = 0;
    
    [Range(0, double.MaxValue)]
    public decimal ValorFechamento { get; set; } = 0;
    
    [Range(0, double.MaxValue)]
    public decimal TotalVendas { get; set; } = 0;
    
    [Range(0, double.MaxValue)]
    public decimal TotalSangrias { get; set; } = 0;
    
    [Range(0, double.MaxValue)]
    public decimal TotalSuprimentos { get; set; } = 0;
    
    [StringLength(500)]
    public string? ObservacoesAbertura { get; set; }
    
    [StringLength(500)]
    public string? ObservacoesFechamento { get; set; }
    
    public int OperadorAberturaId { get; set; }
    
    public int? OperadorFechamentoId { get; set; }
    
    // Relacionamentos
    public virtual Funcionario OperadorAbertura { get; set; } = null!;
    public virtual Funcionario? OperadorFechamento { get; set; }
    public virtual ICollection<MovimentoCaixa> Movimentos { get; set; } = new List<MovimentoCaixa>();
}