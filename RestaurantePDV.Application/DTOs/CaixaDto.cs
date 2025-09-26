using System.ComponentModel.DataAnnotations;
using RestaurantePDV.Domain.Enums;

namespace RestaurantePDV.Application.DTOs;

public class CaixaCreateDto
{
    [Required(ErrorMessage = "Valor de abertura � obrigat�rio")]
    [Range(0, double.MaxValue, ErrorMessage = "Valor de abertura deve ser positivo")]
    public decimal ValorAbertura { get; set; }
    
    [StringLength(500, ErrorMessage = "Observa��es deve ter no m�ximo 500 caracteres")]
    public string? ObservacoesAbertura { get; set; }
    
    [Required(ErrorMessage = "ID do operador � obrigat�rio")]
    public int OperadorAberturaId { get; set; }
}

public class CaixaFechamentoDto
{
    [Required(ErrorMessage = "ID do caixa � obrigat�rio")]
    public int CaixaId { get; set; }
    
    [Required(ErrorMessage = "Valor de fechamento � obrigat�rio")]
    [Range(0, double.MaxValue, ErrorMessage = "Valor de fechamento deve ser positivo")]
    public decimal ValorFechamento { get; set; }
    
    [StringLength(500, ErrorMessage = "Observa��es deve ter no m�ximo 500 caracteres")]
    public string? ObservacoesFechamento { get; set; }
    
    [Required(ErrorMessage = "ID do operador � obrigat�rio")]
    public int OperadorFechamentoId { get; set; }
}

public class CaixaDto
{
    public int Id { get; set; }
    public DateTime DataAbertura { get; set; }
    public DateTime? DataFechamento { get; set; }
    public StatusCaixa Status { get; set; }
    public string StatusDescricao => Status.ToString();
    public decimal ValorAbertura { get; set; }
    public decimal ValorFechamento { get; set; }
    public decimal TotalVendas { get; set; }
    public decimal TotalSangrias { get; set; }
    public decimal TotalSuprimentos { get; set; }
    public string? ObservacoesAbertura { get; set; }
    public string? ObservacoesFechamento { get; set; }
    public int OperadorAberturaId { get; set; }
    public string OperadorAberturaNome { get; set; } = string.Empty;
    public int? OperadorFechamentoId { get; set; }
    public string? OperadorFechamentoNome { get; set; }
    public DateTime CriadoEm { get; set; }
    
    // Propriedades calculadas
    public decimal SaldoTeorico => ValorAbertura + TotalVendas + TotalSuprimentos - TotalSangrias;
    public decimal Diferenca => ValorFechamento - SaldoTeorico;
    public bool TemDiferenca => Math.Abs(Diferenca) > 0.01m;
}

public class CaixaListDto
{
    public int Id { get; set; }
    public DateTime DataAbertura { get; set; }
    public DateTime? DataFechamento { get; set; }
    public StatusCaixa Status { get; set; }
    public string StatusDescricao => Status.ToString();
    public decimal ValorAbertura { get; set; }
    public decimal ValorFechamento { get; set; }
    public decimal TotalVendas { get; set; }
    public string OperadorAberturaNome { get; set; } = string.Empty;
    public string? OperadorFechamentoNome { get; set; }
    
    // Propriedades calculadas
    public decimal SaldoTeorico => ValorAbertura + TotalVendas + TotalSuprimentos - TotalSangrias;
    public decimal TotalSuprimentos { get; set; }
    public decimal TotalSangrias { get; set; }
}

public class MovimentoCaixaCreateDto
{
    [Required(ErrorMessage = "ID do caixa � obrigat�rio")]
    public int CaixaId { get; set; }
    
    [Required(ErrorMessage = "Tipo de movimento � obrigat�rio")]
    public TipoMovimentoCaixa TipoMovimento { get; set; }
    
    [Required(ErrorMessage = "Valor � obrigat�rio")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Valor deve ser maior que zero")]
    public decimal Valor { get; set; }
    
    [Required(ErrorMessage = "Descri��o � obrigat�ria")]
    [StringLength(200, ErrorMessage = "Descri��o deve ter no m�ximo 200 caracteres")]
    public string Descricao { get; set; } = string.Empty;
    
    [StringLength(500, ErrorMessage = "Observa��es deve ter no m�ximo 500 caracteres")]
    public string? Observacoes { get; set; }
    
    public FormaPagamento? FormaPagamento { get; set; }
    
    public int? VendaId { get; set; }
    
    [Required(ErrorMessage = "ID do operador � obrigat�rio")]
    public int OperadorId { get; set; }
    
    [StringLength(100, ErrorMessage = "N�mero do documento deve ter no m�ximo 100 caracteres")]
    public string? NumeroDocumento { get; set; }
}

public class MovimentoCaixaDto
{
    public int Id { get; set; }
    public int CaixaId { get; set; }
    public DateTime DataMovimento { get; set; }
    public TipoMovimentoCaixa TipoMovimento { get; set; }
    public string TipoMovimentoDescricao => TipoMovimento.ToString();
    public decimal Valor { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public string? Observacoes { get; set; }
    public FormaPagamento? FormaPagamento { get; set; }
    public string? FormaPagamentoDescricao => FormaPagamento?.ToString();
    public int? VendaId { get; set; }
    public string? VendaNumero { get; set; }
    public int OperadorId { get; set; }
    public string OperadorNome { get; set; } = string.Empty;
    public string? NumeroDocumento { get; set; }
    public DateTime CriadoEm { get; set; }
}