using System.ComponentModel.DataAnnotations;
using RestaurantePDV.Domain.Enums;

namespace RestaurantePDV.Application.DTOs;

public class VendaCreateDto
{
    public int? ClienteId { get; set; }
    
    [Required(ErrorMessage = "Forma de pagamento é obrigatória")]
    public FormaPagamento FormaPagamento { get; set; }
    
    [Range(0, double.MaxValue, ErrorMessage = "Desconto deve ser positivo")]
    public decimal Desconto { get; set; } = 0;
    
    [Range(0, double.MaxValue, ErrorMessage = "Acréscimo deve ser positivo")]
    public decimal Acrescimo { get; set; } = 0;
    
    [StringLength(500, ErrorMessage = "Observações deve ter no máximo 500 caracteres")]
    public string? Observacoes { get; set; }
    
    public bool VendaBalcao { get; set; } = true;
    
    public int? ComandaId { get; set; }
    
    [Required(ErrorMessage = "Itens da venda são obrigatórios")]
    public List<ItemVendaCreateDto> Itens { get; set; } = new();
}

public class VendaDto
{
    public int Id { get; set; }
    public string NumeroVenda { get; set; } = string.Empty;
    public int? ClienteId { get; set; }
    public string? ClienteNome { get; set; }
    public DateTime DataVenda { get; set; }
    public StatusVenda Status { get; set; }
    public decimal SubTotal { get; set; }
    public decimal Desconto { get; set; }
    public decimal Acrescimo { get; set; }
    public decimal ValorTotal { get; set; }
    public FormaPagamento FormaPagamento { get; set; }
    public decimal ValorPago { get; set; }
    public decimal Troco { get; set; }
    public string? Observacoes { get; set; }
    public bool VendaBalcao { get; set; }
    public int? ComandaId { get; set; }
    public List<ItemVendaDto> Itens { get; set; } = new();
}

public class ItemVendaCreateDto
{
    [Required(ErrorMessage = "ID do produto é obrigatório")]
    public int ProdutoId { get; set; }
    
    [Required(ErrorMessage = "Quantidade é obrigatória")]
    [Range(0.001, double.MaxValue, ErrorMessage = "Quantidade deve ser maior que zero")]
    public decimal Quantidade { get; set; }
    
    [Range(0, double.MaxValue, ErrorMessage = "Desconto deve ser positivo")]
    public decimal Desconto { get; set; } = 0;
    
    [StringLength(500, ErrorMessage = "Observações deve ter no máximo 500 caracteres")]
    public string? Observacoes { get; set; }
    
    [StringLength(500, ErrorMessage = "Adicionais deve ter no máximo 500 caracteres")]
    public string? Adicionais { get; set; }
}

public class ItemVendaDto
{
    public int Id { get; set; }
    public int VendaId { get; set; }
    public int ProdutoId { get; set; }
    public string ProdutoNome { get; set; } = string.Empty;
    public decimal Quantidade { get; set; }
    public decimal ValorUnitario { get; set; }
    public decimal Desconto { get; set; }
    public decimal ValorTotal { get; set; }
    public string? Observacoes { get; set; }
    public string? Adicionais { get; set; }
}