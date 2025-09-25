using System.ComponentModel.DataAnnotations;
using RestaurantePDV.Domain.Enums;

namespace RestaurantePDV.Application.DTOs;

public class ClienteRestricaoCreateDto
{
    [Required(ErrorMessage = "ID do cliente é obrigatório")]
    public int ClienteId { get; set; }
    
    [Required(ErrorMessage = "Motivo da restrição é obrigatório")]
    public MotivoRestricaoCliente Motivo { get; set; }
    
    [Required(ErrorMessage = "Descrição é obrigatória")]
    [StringLength(500, ErrorMessage = "Descrição deve ter no máximo 500 caracteres")]
    public string Descricao { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "ID do responsável é obrigatório")]
    public int ResponsavelInclusaoId { get; set; }
}

public class ClienteRestricaoRemoverDto
{
    [Required(ErrorMessage = "ID da restrição é obrigatório")]
    public int Id { get; set; }
    
    [Required(ErrorMessage = "ID do responsável pela remoção é obrigatório")]
    public int ResponsavelRemocaoId { get; set; }
    
    [StringLength(500, ErrorMessage = "Observações deve ter no máximo 500 caracteres")]
    public string? ObservacoesRemocao { get; set; }
}

public class ClienteRestricaoDto
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public string ClienteNome { get; set; } = string.Empty;
    public MotivoRestricaoCliente Motivo { get; set; }
    public string MotivoDescricao => Motivo.ToString();
    public string Descricao { get; set; } = string.Empty;
    public DateTime DataInclusao { get; set; }
    public DateTime? DataRemocao { get; set; }
    public int ResponsavelInclusaoId { get; set; }
    public string ResponsavelInclusaoNome { get; set; } = string.Empty;
    public int? ResponsavelRemocaoId { get; set; }
    public string? ResponsavelRemocaoNome { get; set; }
    public string? ObservacoesRemocao { get; set; }
    public bool Ativa { get; set; }
    public DateTime CriadoEm { get; set; }
    public DateTime? AtualizadoEm { get; set; }
}

public class ClienteRestricaoListDto
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public string ClienteNome { get; set; } = string.Empty;
    public MotivoRestricaoCliente Motivo { get; set; }
    public string MotivoDescricao => Motivo.ToString();
    public string Descricao { get; set; } = string.Empty;
    public DateTime DataInclusao { get; set; }
    public string ResponsavelInclusaoNome { get; set; } = string.Empty;
    public bool Ativa { get; set; }
}

public class ClienteComRestricaoDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? CpfCnpj { get; set; }
    public string? Telefone { get; set; }
    public string? Email { get; set; }
    public bool TemRestricaoAtiva { get; set; }
    public List<ClienteRestricaoDto> Restricoes { get; set; } = new();
}