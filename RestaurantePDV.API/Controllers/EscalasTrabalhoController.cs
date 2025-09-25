using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantePDV.Application.DTOs;
using RestaurantePDV.Application.Services;
using RestaurantePDV.Domain.Enums;

namespace RestaurantePDV.API.Controllers;

[ApiController]
[Route("api/escalas-trabalho")]
[Authorize]
public class EscalasTrabalhoController : ControllerBase
{
    private readonly IEscalaTrabalhoService _escalaService;
    private readonly ILogger<EscalasTrabalhoController> _logger;

    public EscalasTrabalhoController(IEscalaTrabalhoService escalaService, ILogger<EscalasTrabalhoController> logger)
    {
        _escalaService = escalaService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<EscalaTrabalhoListDto>>> GetAll()
    {
        try
        {
            var escalas = await _escalaService.GetAllAsync();
            return Ok(escalas);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar escalas de trabalho");
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<EscalaTrabalhoDto>> GetById(int id)
    {
        try
        {
            var escala = await _escalaService.GetByIdAsync(id);
            return Ok(escala);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar escala por ID: {Id}", id);
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpGet("funcionario/{funcionarioId}")]
    public async Task<ActionResult<IEnumerable<EscalaTrabalhoListDto>>> GetByFuncionario(int funcionarioId)
    {
        try
        {
            var escalas = await _escalaService.GetByFuncionarioAsync(funcionarioId);
            return Ok(escalas);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar escalas por funcionário: {FuncionarioId}", funcionarioId);
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpGet("data/{data}")]
    public async Task<ActionResult<IEnumerable<EscalaTrabalhoListDto>>> GetByData(DateTime data)
    {
        try
        {
            var escalas = await _escalaService.GetByDataAsync(data);
            return Ok(escalas);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar escalas por data: {Data}", data);
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpGet("periodo")]
    public async Task<ActionResult<IEnumerable<EscalaTrabalhoListDto>>> GetByPeriodo([FromQuery] DateTime dataInicio, [FromQuery] DateTime dataFim)
    {
        try
        {
            var escalas = await _escalaService.GetByPeriodoAsync(dataInicio, dataFim);
            return Ok(escalas);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar escalas por período: {DataInicio} - {DataFim}", dataInicio, dataFim);
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpGet("turno/{turno}")]
    public async Task<ActionResult<IEnumerable<EscalaTrabalhoListDto>>> GetByTurno(TurnoTrabalho turno)
    {
        try
        {
            var escalas = await _escalaService.GetByTurnoAsync(turno);
            return Ok(escalas);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar escalas por turno: {Turno}", turno);
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpGet("funcionario/{funcionarioId}/data/{data}")]
    public async Task<ActionResult<EscalaTrabalhoDto>> GetByFuncionarioData(int funcionarioId, DateTime data)
    {
        try
        {
            var escala = await _escalaService.GetByFuncionarioDataAsync(funcionarioId, data);
            if (escala == null)
                return NotFound("Escala não encontrada para o funcionário na data especificada");

            return Ok(escala);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar escala por funcionário e data: {FuncionarioId}, {Data}", funcionarioId, data);
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Gerente")]
    public async Task<ActionResult<EscalaTrabalhoDto>> Create([FromBody] EscalaTrabalhoCreateDto escalaDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var escala = await _escalaService.CreateAsync(escalaDto);
            return CreatedAtAction(nameof(GetById), new { id = escala.Id }, escala);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar escala de trabalho");
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Gerente")]
    public async Task<ActionResult<EscalaTrabalhoDto>> Update(int id, [FromBody] EscalaTrabalhoUpdateDto escalaDto)
    {
        if (id != escalaDto.Id)
            return BadRequest("ID da escala não confere");

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var escala = await _escalaService.UpdateAsync(escalaDto);
            return Ok(escala);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar escala de trabalho: {Id}", id);
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,Gerente")]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            await _escalaService.DeleteAsync(id);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao deletar escala de trabalho: {Id}", id);
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpGet("funcionario/{funcionarioId}/verificar-escala/{data}")]
    public async Task<ActionResult<bool>> VerificarEscala(int funcionarioId, DateTime data)
    {
        try
        {
            var temEscala = await _escalaService.FuncionarioTemEscalaAsync(funcionarioId, data);
            return Ok(new { temEscala });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao verificar escala do funcionário: {FuncionarioId}, {Data}", funcionarioId, data);
            return StatusCode(500, "Erro interno do servidor");
        }
    }
}