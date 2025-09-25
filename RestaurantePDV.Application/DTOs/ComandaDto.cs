using System.ComponentModel.DataAnnotations;
using RestaurantePDV.Domain.Enums;

namespace RestaurantePDV.Application.DTOs;

public class ComandaCreateDto
{
    public int? ClienteId { get; set; }
    
    [StringLength(50, ErrorMessage = "Mesa deve ter no máximo 50 caracteres")]
    public string? Mesa { get; set; }
    
    [StringLength(500, ErrorMessage = "Observações deve ter no máximo 500 caracteres")]
    public string? Observacoes { get; set; }
    
    public int? GarcomId { get; set; }
}

public class ComandaUpdateDto
{
    [Required]
    public int Id { get; set; }
    
    public int? ClienteId { get; set; }
    
    [StringLength(50, ErrorMessage = "Mesa deve ter no máximo 50 caracteres")]
    public string? Mesa { get; set; }
    
    [Range(0, double.MaxValue, ErrorMessage = "Desconto deve ser positivo")]
    public decimal Desconto { get; set; } = 0;
    
    [Range(0, double.MaxValue, ErrorMessage = "Acréscimo deve ser positivo")]
    public decimal Acrescimo { get; set; } = 0;
    
    [StringLength(500, ErrorMessage = "Observações deve ter no máximo 500 caracteres")]
    public string? Observacoes { get; set; }
    
    public int? GarcomId { get; set; }
}

public class ComandaDto
{
    public int Id { get; set; }
    public string NumeroComanda { get; set; } = string.Empty;
    public int? ClienteId { get; set; }
    public string? ClienteNome { get; set; }
    public DateTime DataAbertura { get; set; }
    public DateTime? DataFechamento { get; set; }
    public StatusComanda Status { get; set; }
    public string? Mesa { get; set; }
    public decimal ValorTotal { get; set; }
    public decimal Desconto { get; set; }
    public decimal Acrescimo { get; set; }
    public decimal ValorFinal { get; set; }
    public string? Observacoes { get; set; }
    public int? GarcomId { get; set; }
    public string? GarcomNome { get; set; }
    public DateTime CriadoEm { get; set; }
    public List<ItemComandaDto> Itens { get; set; } = new();
}

public class ItemComandaCreateDto
{
    [Required(ErrorMessage = "ID da comanda é obrigatório")]
    public int ComandaId { get; set; }
    
    [Required(ErrorMessage = "ID do produto é obrigatório")]
    public int ProdutoId { get; set; }
    
    [Required(ErrorMessage = "Quantidade é obrigatória")]
    [Range(0.001, double.MaxValue, ErrorMessage = "Quantidade deve ser maior que zero")]
    public decimal Quantidade { get; set; }
    
    [StringLength(500, ErrorMessage = "Observações deve ter no máximo 500 caracteres")]
    public string? Observacoes { get; set; }
    
    [StringLength(500, ErrorMessage = "Adicionais deve ter no máximo 500 caracteres")]
    public string? Adicionais { get; set; }
}

public class ItemComandaDto
{
    public int Id { get; set; }
    public int ComandaId { get; set; }
    public int ProdutoId { get; set; }
    public string ProdutoNome { get; set; } = string.Empty;
    public decimal Quantidade { get; set; }
    public decimal ValorUnitario { get; set; }
    public decimal ValorTotal { get; set; }
    public string? Observacoes { get; set; }
    public string? Adicionais { get; set; }
    public bool Preparado { get; set; }
    public DateTime? DataPreparo { get; set; }
    public bool Entregue { get; set; }
    public DateTime? DataEntrega { get; set; }
}