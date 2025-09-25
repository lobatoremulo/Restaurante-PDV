using System.ComponentModel.DataAnnotations;
using RestaurantePDV.Domain.Enums;

namespace RestaurantePDV.Application.DTOs;

public class ProdutoCreateDto
{
    [Required(ErrorMessage = "Nome é obrigatório")]
    [StringLength(100, ErrorMessage = "Nome deve ter no máximo 100 caracteres")]
    public string Nome { get; set; } = string.Empty;
    
    [StringLength(500, ErrorMessage = "Descrição deve ter no máximo 500 caracteres")]
    public string? Descricao { get; set; }
    
    [Required(ErrorMessage = "Tipo é obrigatório")]
    public TipoProduto Tipo { get; set; }
    
    [Required(ErrorMessage = "Preço de venda é obrigatório")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Preço de venda deve ser maior que zero")]
    public decimal PrecoVenda { get; set; }
    
    [Range(0, double.MaxValue, ErrorMessage = "Preço de custo deve ser positivo")]
    public decimal? PrecoCusto { get; set; }
    
    [StringLength(50, ErrorMessage = "Código de barras deve ter no máximo 50 caracteres")]
    public string? CodigoBarras { get; set; }
    
    [StringLength(20, ErrorMessage = "Unidade deve ter no máximo 20 caracteres")]
    public string? Unidade { get; set; } = "UN";
    
    public bool ControlaNaoEstoque { get; set; } = false;
    
    [Range(0, double.MaxValue, ErrorMessage = "Estoque mínimo deve ser positivo")]
    public decimal EstoqueMinimo { get; set; } = 0;
    
    [Range(0, double.MaxValue, ErrorMessage = "Estoque atual deve ser positivo")]
    public decimal EstoqueAtual { get; set; } = 0;
    
    [StringLength(200, ErrorMessage = "Caminho da foto deve ter no máximo 200 caracteres")]
    public string? CaminhoFoto { get; set; }
    
    [StringLength(500, ErrorMessage = "Ingredientes deve ter no máximo 500 caracteres")]
    public string? Ingredientes { get; set; }
    
    [Range(0, int.MaxValue, ErrorMessage = "Tempo de preparo deve ser positivo")]
    public int? TempoPreparoMinutos { get; set; }
    
    public bool DisponivelDelivery { get; set; } = true;
}

public class ProdutoUpdateDto : ProdutoCreateDto
{
    [Required]
    public int Id { get; set; }
}

public class ProdutoDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public TipoProduto Tipo { get; set; }
    public decimal PrecoVenda { get; set; }
    public decimal? PrecoCusto { get; set; }
    public string? CodigoBarras { get; set; }
    public string? Unidade { get; set; }
    public bool ControlaNaoEstoque { get; set; }
    public decimal EstoqueMinimo { get; set; }
    public decimal EstoqueAtual { get; set; }
    public string? CaminhoFoto { get; set; }
    public string? Ingredientes { get; set; }
    public int? TempoPreparoMinutos { get; set; }
    public bool DisponivelDelivery { get; set; }
    public DateTime CriadoEm { get; set; }
    public DateTime? AtualizadoEm { get; set; }
    public bool Ativo { get; set; }
}