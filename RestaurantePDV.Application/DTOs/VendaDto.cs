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

// Novos DTOs para pagamento de comandas
public class PagamentoComandaDto
{
    [Required(ErrorMessage = "ID da comanda é obrigatório")]
    public int ComandaId { get; set; }
    
    [Range(0, double.MaxValue, ErrorMessage = "Desconto deve ser positivo")]
    public decimal Desconto { get; set; } = 0;
    
    [Range(0, double.MaxValue, ErrorMessage = "Acréscimo deve ser positivo")]
    public decimal Acrescimo { get; set; } = 0;
    
    [StringLength(500, ErrorMessage = "Observações deve ter no máximo 500 caracteres")]
    public string? Observacoes { get; set; }
    
    [Required(ErrorMessage = "Pelo menos uma forma de pagamento é obrigatória")]
    public List<FormaPagamentoDto> FormasPagamento { get; set; } = new();
    
    [Required(ErrorMessage = "ID do operador é obrigatório")]
    public int OperadorId { get; set; }
    
    // Para dividir a conta (opcional)
    public List<DivisaoContaDto>? DivisoesConta { get; set; }
}

public class FormaPagamentoDto
{
    [Required(ErrorMessage = "Forma de pagamento é obrigatória")]
    public FormaPagamento FormaPagamento { get; set; }
    
    [Required(ErrorMessage = "Valor é obrigatório")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Valor deve ser maior que zero")]
    public decimal Valor { get; set; }
    
    // Para pagamentos em dinheiro
    public decimal? ValorRecebido { get; set; }
    
    [StringLength(100, ErrorMessage = "Número do documento deve ter no máximo 100 caracteres")]
    public string? NumeroDocumento { get; set; }
    
    [StringLength(200, ErrorMessage = "Observações deve ter no máximo 200 caracteres")]
    public string? Observacoes { get; set; }
}

public class DivisaoContaDto
{
    [Required(ErrorMessage = "Descrição é obrigatória")]
    [StringLength(100, ErrorMessage = "Descrição deve ter no máximo 100 caracteres")]
    public string Descricao { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Valor é obrigatório")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Valor deve ser maior que zero")]
    public decimal Valor { get; set; }
    
    public int? ClienteId { get; set; }
    
    [StringLength(200, ErrorMessage = "Observações deve ter no máximo 200 caracteres")]
    public string? Observacoes { get; set; }
}

public class VendaComandaDto
{
    public int Id { get; set; }
    public string NumeroVenda { get; set; } = string.Empty;
    public int ComandaId { get; set; }
    public string NumeroComanda { get; set; } = string.Empty;
    public DateTime DataVenda { get; set; }
    public StatusVenda Status { get; set; }
    public decimal SubTotal { get; set; }
    public decimal Desconto { get; set; }
    public decimal Acrescimo { get; set; }
    public decimal ValorTotal { get; set; }
    public decimal ValorPago { get; set; }
    public decimal Troco { get; set; }
    public string? Observacoes { get; set; }
    public string OperadorNome { get; set; } = string.Empty;
    public List<PagamentoVendaDto> Pagamentos { get; set; } = new();
    public List<ItemVendaDto> Itens { get; set; } = new();
    
    // Propriedades calculadas
    public bool TemTroco => Troco > 0;
    public bool PagamentoCompleto => ValorPago >= ValorTotal;
}

public class PagamentoVendaDto
{
    public int Id { get; set; }
    public FormaPagamento FormaPagamento { get; set; }
    public string FormaPagamentoDescricao => FormaPagamento.ToString();
    public decimal Valor { get; set; }
    public decimal? ValorRecebido { get; set; }
    public decimal Troco => ValorRecebido.HasValue ? Math.Max(0, ValorRecebido.Value - Valor) : 0;
    public string? NumeroDocumento { get; set; }
    public string? Observacoes { get; set; }
    public DateTime DataPagamento { get; set; }
}

public class ComandaParaPagamentoDto
{
    public int Id { get; set; }
    public string NumeroComanda { get; set; } = string.Empty;
    public string? Mesa { get; set; }
    public string? ClienteNome { get; set; }
    public string? GarcomNome { get; set; }
    public decimal ValorTotal { get; set; }
    public decimal Desconto { get; set; }
    public decimal Acrescimo { get; set; }
    public decimal ValorFinal { get; set; }
    public DateTime DataAbertura { get; set; }
    public List<ItemComandaResumoDto> Itens { get; set; } = new();
    
    // Verificações
    public bool PodeSerPaga => ValorFinal > 0;
}

public class ItemComandaResumoDto
{
    public int Id { get; set; }
    public string ProdutoNome { get; set; } = string.Empty;
    public decimal Quantidade { get; set; }
    public decimal ValorUnitario { get; set; }
    public decimal ValorTotal { get; set; }
    public string? Observacoes { get; set; }
    public string? Adicionais { get; set; }
}