using System.ComponentModel.DataAnnotations;

namespace RestaurantePDV.Application.DTOs;

public class ClienteCreateDto
{
    [Required(ErrorMessage = "Nome é obrigatório")]
    [StringLength(100, ErrorMessage = "Nome deve ter no máximo 100 caracteres")]
    public string Nome { get; set; } = string.Empty;
    
    [StringLength(14, ErrorMessage = "CPF/CNPJ deve ter no máximo 14 caracteres")]
    public string? CpfCnpj { get; set; }
    
    [StringLength(20, ErrorMessage = "Telefone deve ter no máximo 20 caracteres")]
    public string? Telefone { get; set; }
    
    [EmailAddress(ErrorMessage = "E-mail inválido")]
    [StringLength(100, ErrorMessage = "E-mail deve ter no máximo 100 caracteres")]
    public string? Email { get; set; }
    
    [StringLength(200, ErrorMessage = "Endereço deve ter no máximo 200 caracteres")]
    public string? Endereco { get; set; }
    
    [StringLength(50, ErrorMessage = "Cidade deve ter no máximo 50 caracteres")]
    public string? Cidade { get; set; }
    
    [StringLength(2, ErrorMessage = "Estado deve ter 2 caracteres")]
    public string? Estado { get; set; }
    
    [StringLength(9, ErrorMessage = "CEP deve ter no máximo 9 caracteres")]
    public string? Cep { get; set; }
    
    public DateTime? DataNascimento { get; set; }
    
    [Range(0, double.MaxValue, ErrorMessage = "Limite de crédito deve ser positivo")]
    public decimal LimiteCredito { get; set; } = 0;
    
    [StringLength(500, ErrorMessage = "Observações deve ter no máximo 500 caracteres")]
    public string? Observacoes { get; set; }
}

public class ClienteUpdateDto : ClienteCreateDto
{
    [Required]
    public int Id { get; set; }
}

public class ClienteDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? CpfCnpj { get; set; }
    public string? Telefone { get; set; }
    public string? Email { get; set; }
    public string? Endereco { get; set; }
    public string? Cidade { get; set; }
    public string? Estado { get; set; }
    public string? Cep { get; set; }
    public DateTime? DataNascimento { get; set; }
    public decimal LimiteCredito { get; set; }
    public string? Observacoes { get; set; }
    public DateTime CriadoEm { get; set; }
    public DateTime? AtualizadoEm { get; set; }
    public bool Ativo { get; set; }
    
    // Propriedades relacionadas a restrições
    public bool TemRestricaoAtiva { get; set; }
    public int TotalVendas { get; set; }
    public decimal ValorTotalCompras { get; set; }
    public DateTime? UltimaCompra { get; set; }
    public List<ClienteRestricaoDto> Restricoes { get; set; } = new();
}

public class ClienteListDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? CpfCnpj { get; set; }
    public string? Telefone { get; set; }
    public string? Email { get; set; }
    public decimal LimiteCredito { get; set; }
    public bool TemRestricaoAtiva { get; set; }
    public int TotalVendas { get; set; }
    public decimal ValorTotalCompras { get; set; }
    public DateTime? UltimaCompra { get; set; }
    public bool Ativo { get; set; }
}