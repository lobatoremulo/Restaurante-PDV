using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantePDV.Application.DTOs;
using RestaurantePDV.Application.Services;
using RestaurantePDV.Domain.Enums;

namespace RestaurantePDV.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CaixaController(ICaixaService caixaService, ILogger<CaixaController> logger) : ControllerBase
{
    private readonly ICaixaService _caixaService = caixaService;
    private readonly ILogger<CaixaController> _logger = logger;

    [HttpGet]
    [Authorize(Roles = "Admin,Gerente")]
    public async Task<ActionResult<IEnumerable<CaixaListDto>>> GetAll()
    {
        try
        {
            var caixas = await _caixaService.GetAllAsync();
            return Ok(caixas);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar caixas");
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpGet("aberto")]
    public async Task<ActionResult<CaixaDto>> GetCaixaAberto()
    {
        try
        {
            var caixa = await _caixaService.GetCaixaAbertoAsync();
            if (caixa == null)
                return NotFound(new { message = "Nenhum caixa aberto encontrado" });

            return Ok(caixa);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar caixa aberto");
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpGet("status")]
    public async Task<ActionResult<object>> GetStatusCaixa()
    {
        try
        {
            var temCaixaAberto = await _caixaService.TemCaixaAbertoAsync();
            var caixaAberto = temCaixaAberto ? await _caixaService.GetCaixaAbertoAsync() : null;

            return Ok(new
            {
                TemCaixaAberto = temCaixaAberto,
                CaixaAberto = caixaAberto
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao verificar status do caixa");
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Gerente,UsuarioComum")]
    public async Task<ActionResult<CaixaDto>> GetById(int id)
    {
        try
        {
            var caixa = await _caixaService.GetByIdAsync(id);
            return Ok(caixa);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar caixa por ID: {Id}", id);
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpGet("periodo")]
    [Authorize(Roles = "Admin,Gerente")]
    public async Task<ActionResult<IEnumerable<CaixaListDto>>> GetByPeriodo(
        [FromQuery] DateTime dataInicio, 
        [FromQuery] DateTime dataFim)
    {
        try
        {
            if (dataInicio > dataFim)
                return BadRequest("Data de início deve ser menor ou igual à data fim");

            var caixas = await _caixaService.GetByPeriodoAsync(dataInicio, dataFim);
            return Ok(caixas);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar caixas por período: {DataInicio} - {DataFim}", dataInicio, dataFim);
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpPost("abrir")]
    [Authorize(Roles = "Admin,Gerente,UsuarioComum")]
    public async Task<ActionResult<CaixaDto>> AbrirCaixa([FromBody] CaixaCreateDto caixaDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var caixa = await _caixaService.AbrirCaixaAsync(caixaDto);
            return CreatedAtAction(nameof(GetById), new { id = caixa.Id }, caixa);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao abrir caixa");
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpPost("fechar")]
    [Authorize(Roles = "Admin,Gerente,UsuarioComum")]
    public async Task<ActionResult<CaixaDto>> FecharCaixa([FromBody] CaixaFechamentoDto fechamentoDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var caixa = await _caixaService.FecharCaixaAsync(fechamentoDto);
            return Ok(caixa);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao fechar caixa: {CaixaId}", fechamentoDto.CaixaId);
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpGet("{id}/relatorio")]
    [Authorize(Roles = "Admin,Gerente")]
    public async Task<ActionResult<object>> GetRelatorioCaixa(int id)
    {
        try
        {
            var relatorio = await _caixaService.GetRelatorioCaixaAsync(id);
            return Ok(relatorio);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao gerar relatório do caixa: {Id}", id);
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpGet("relatorio-financeiro")]
    [Authorize(Roles = "Admin,Gerente")]
    public async Task<ActionResult<object>> GetRelatorioFinanceiro(
        [FromQuery] DateTime dataInicio, 
        [FromQuery] DateTime dataFim)
    {
        try
        {
            if (dataInicio > dataFim)
                return BadRequest("Data de início deve ser menor ou igual à data fim");

            var relatorio = await _caixaService.GetRelatorioFinanceiroAsync(dataInicio, dataFim);
            return Ok(relatorio);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao gerar relatório financeiro: {DataInicio} - {DataFim}", dataInicio, dataFim);
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpGet("tipos-movimento")]
    public ActionResult<object> GetTiposMovimento()
    {
        try
        {
            var tipos = Enum.GetValues<TipoMovimentoCaixa>()
                .Select(t => new { value = (int)t, name = t.ToString() })
                .ToList();
            
            return Ok(tipos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar tipos de movimento");
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpGet("status-caixa")]
    public ActionResult<object> GetStatusCaixaEnum()
    {
        try
        {
            var status = Enum.GetValues<StatusCaixa>()
                .Select(s => new { value = (int)s, name = s.ToString() })
                .ToList();
            
            return Ok(status);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar status de caixa");
            return StatusCode(500, "Erro interno do servidor");
        }
    }
}