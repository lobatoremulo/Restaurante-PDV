using System.ComponentModel.DataAnnotations;
using RestaurantePDV.Domain.Enums;

namespace RestaurantePDV.Application.DTOs;

public class FuncionarioCreateDto
{
    [Required(ErrorMessage = "Nome � obrigat�rio")]
    [StringLength(100, ErrorMessage = "Nome deve ter no m�ximo 100 caracteres")]
    public string Nome { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "CPF � obrigat�rio")]
    [StringLength(14, ErrorMessage = "CPF deve ter no m�ximo 14 caracteres")]
    public string Cpf { get; set; } = string.Empty;
    
    [StringLength(15, ErrorMessage = "RG deve ter no m�ximo 15 caracteres")]
    public string? Rg { get; set; }
    
    [StringLength(20, ErrorMessage = "Telefone deve ter no m�ximo 20 caracteres")]
    public string? Telefone { get; set; }
    
    [EmailAddress(ErrorMessage = "E-mail inv�lido")]
    [StringLength(100, ErrorMessage = "E-mail deve ter no m�ximo 100 caracteres")]
    public string? Email { get; set; }
    
    [Required(ErrorMessage = "Cargo � obrigat�rio")]
    [StringLength(100, ErrorMessage = "Cargo deve ter no m�ximo 100 caracteres")]
    public string Cargo { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Setor � obrigat�rio")]
    [StringLength(100, ErrorMessage = "Setor deve ter no m�ximo 100 caracteres")]
    public string Setor { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "N�vel de acesso � obrigat�rio")]
    public NivelAcesso NivelAcesso { get; set; }
    
    public StatusFuncionario Status { get; set; } = StatusFuncionario.Ativo;
    
    public DateTime? DataAdmissao { get; set; }
    
    [Range(0, double.MaxValue, ErrorMessage = "Sal�rio deve ser positivo")]
    public decimal? Salario { get; set; }
    
    [StringLength(200, ErrorMessage = "Endere�o deve ter no m�ximo 200 caracteres")]
    public string? Endereco { get; set; }
    
    [StringLength(50, ErrorMessage = "Cidade deve ter no m�ximo 50 caracteres")]
    public string? Cidade { get; set; }
    
    [StringLength(2, ErrorMessage = "Estado deve ter 2 caracteres")]
    public string? Estado { get; set; }
    
    [StringLength(9, ErrorMessage = "CEP deve ter no m�ximo 9 caracteres")]
    public string? Cep { get; set; }
    
    public DateTime? DataNascimento { get; set; }
    
    [StringLength(500, ErrorMessage = "Observa��es deve ter no m�ximo 500 caracteres")]
    public string? Observacoes { get; set; }
}

public class FuncionarioUpdateDto : FuncionarioCreateDto
{
    [Required]
    public int Id { get; set; }
    
    public DateTime? DataDemissao { get; set; }
}

public class FuncionarioDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public string? Rg { get; set; }
    public string? Telefone { get; set; }
    public string? Email { get; set; }
    public string Cargo { get; set; } = string.Empty;
    public string Setor { get; set; } = string.Empty;
    public NivelAcesso NivelAcesso { get; set; }
    public string NivelAcessoDescricao => NivelAcesso.ToString();
    public StatusFuncionario Status { get; set; }
    public string StatusDescricao => Status.ToString();
    public DateTime? DataAdmissao { get; set; }
    public DateTime? DataDemissao { get; set; }
    public decimal? Salario { get; set; }
    public string? Endereco { get; set; }
    public string? Cidade { get; set; }
    public string? Estado { get; set; }
    public string? Cep { get; set; }
    public DateTime? DataNascimento { get; set; }
    public string? Observacoes { get; set; }
    public DateTime CriadoEm { get; set; }
    public DateTime? AtualizadoEm { get; set; }
    public bool Ativo { get; set; }
}

public class FuncionarioListDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public string Cargo { get; set; } = string.Empty;
    public string Setor { get; set; } = string.Empty;
    public NivelAcesso NivelAcesso { get; set; }
    public string NivelAcessoDescricao => NivelAcesso.ToString();
    public StatusFuncionario Status { get; set; }
    public string StatusDescricao => Status.ToString();
    public DateTime? DataAdmissao { get; set; }
    public bool Ativo { get; set; }
}