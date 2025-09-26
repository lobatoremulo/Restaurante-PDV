using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantePDV.Application.DTOs;
using RestaurantePDV.Application.Services;

namespace RestaurantePDV.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MovimentoCaixaController(IMovimentoCaixaService movimentoCaixaService, ILogger<MovimentoCaixaController> logger) : ControllerBase
{
    private readonly IMovimentoCaixaService _movimentoCaixaService = movimentoCaixaService;
    private readonly ILogger<MovimentoCaixaController> _logger = logger;

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Gerente")]
    public async Task<ActionResult<MovimentoCaixaDto>> GetById(int id)
    {
        try
        {
            var movimento = await _movimentoCaixaService.GetByIdAsync(id);
            return Ok(movimento);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar movimento por ID: {Id}", id);
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpGet("caixa/{caixaId}")]
    [Authorize(Roles = "Admin,Gerente,UsuarioComum")]
    public async Task<ActionResult<IEnumerable<MovimentoCaixaDto>>> GetByCaixa(int caixaId)
    {
        try
        {
            var movimentos = await _movimentoCaixaService.GetByCaixaAsync(caixaId);
            return Ok(movimentos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar movimentos por caixa: {CaixaId}", caixaId);
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpGet("periodo")]
    [Authorize(Roles = "Admin,Gerente")]
    public async Task<ActionResult<IEnumerable<MovimentoCaixaDto>>> GetByPeriodo(
        [FromQuery] DateTime dataInicio, 
        [FromQuery] DateTime dataFim)
    {
        try
        {
            if (dataInicio > dataFim)
                return BadRequest("Data de início deve ser menor ou igual à data fim");

            var movimentos = await _movimentoCaixaService.GetByPeriodoAsync(dataInicio, dataFim);
            return Ok(movimentos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar movimentos por período: {DataInicio} - {DataFim}", dataInicio, dataFim);
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Gerente,UsuarioComum")]
    public async Task<ActionResult<MovimentoCaixaDto>> Create([FromBody] MovimentoCaixaCreateDto movimentoDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var movimento = await _movimentoCaixaService.AdicionarMovimentoAsync(movimentoDto);
            return CreatedAtAction(nameof(GetById), new { id = movimento.Id }, movimento);
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
            _logger.LogError(ex, "Erro ao criar movimento de caixa");
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpPost("registrar-venda")]
    [Authorize(Roles = "Admin,Gerente,UsuarioComum")]
    public async Task<ActionResult<MovimentoCaixaDto>> RegistrarVenda([FromBody] object request)
    {
        try
        {
            // Extrair dados do request
            var requestData = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(request.ToString()!);
            
            if (!requestData.TryGetValue("vendaId", out var vendaIdObj) ||
                !requestData.TryGetValue("caixaId", out var caixaIdObj) ||
                !requestData.TryGetValue("operadorId", out var operadorIdObj))
            {
                return BadRequest("VendaId, CaixaId e OperadorId são obrigatórios");
            }

            var vendaId = Convert.ToInt32(vendaIdObj);
            var caixaId = Convert.ToInt32(caixaIdObj);
            var operadorId = Convert.ToInt32(operadorIdObj);

            var movimento = await _movimentoCaixaService.RegistrarVendaAsync(vendaId, caixaId, operadorId);
            return CreatedAtAction(nameof(GetById), new { id = movimento.Id }, movimento);
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
            _logger.LogError(ex, "Erro ao registrar venda no caixa");
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpPost("sangria")]
    [Authorize(Roles = "Admin,Gerente")]
    public async Task<ActionResult<MovimentoCaixaDto>> RegistrarSangria([FromBody] MovimentoCaixaCreateDto sangriaDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var movimento = await _movimentoCaixaService.RegistrarSangriaAsync(sangriaDto);
            return CreatedAtAction(nameof(GetById), new { id = movimento.Id }, movimento);
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
            _logger.LogError(ex, "Erro ao registrar sangria");
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpPost("suprimento")]
    [Authorize(Roles = "Admin,Gerente,UsuarioComum")]
    public async Task<ActionResult<MovimentoCaixaDto>> RegistrarSuprimento([FromBody] MovimentoCaixaCreateDto suprimentoDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var movimento = await _movimentoCaixaService.RegistrarSuprimentoAsync(suprimentoDto);
            return CreatedAtAction(nameof(GetById), new { id = movimento.Id }, movimento);
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
            _logger.LogError(ex, "Erro ao registrar suprimento");
            return StatusCode(500, "Erro interno do servidor");
        }
    }
}