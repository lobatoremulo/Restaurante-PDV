using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantePDV.Application.DTOs;
using RestaurantePDV.Application.Services;
using RestaurantePDV.Domain.Enums;

namespace RestaurantePDV.API.Controllers;

[ApiController]
[Route("api/clientes-restricoes")]
[Authorize]
public class ClientesRestricoesController : ControllerBase
{
    private readonly IClienteRestricaoService _restricaoService;
    private readonly ILogger<ClientesRestricoesController> _logger;

    public ClientesRestricoesController(IClienteRestricaoService restricaoService, ILogger<ClientesRestricoesController> logger)
    {
        _restricaoService = restricaoService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ClienteRestricaoListDto>>> GetAll()
    {
        try
        {
            var restricoes = await _restricaoService.GetAllAsync();
            return Ok(restricoes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar restri��es de clientes");
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpGet("ativas")]
    public async Task<ActionResult<IEnumerable<ClienteRestricaoListDto>>> GetAtivas()
    {
        try
        {
            var restricoes = await _restricaoService.GetRestricoesAtivasAsync();
            return Ok(restricoes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar restri��es ativas");
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpGet("clientes-com-restricao")]
    public async Task<ActionResult<IEnumerable<ClienteComRestricaoDto>>> GetClientesComRestricao()
    {
        try
        {
            var clientes = await _restricaoService.GetClientesComRestricaoAsync();
            return Ok(clientes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar clientes com restri��o");
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ClienteRestricaoDto>> GetById(int id)
    {
        try
        {
            var restricao = await _restricaoService.GetByIdAsync(id);
            return Ok(restricao);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar restri��o por ID: {Id}", id);
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpGet("cliente/{clienteId}")]
    public async Task<ActionResult<IEnumerable<ClienteRestricaoListDto>>> GetByCliente(int clienteId)
    {
        try
        {
            var restricoes = await _restricaoService.GetByClienteAsync(clienteId);
            return Ok(restricoes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar restri��es por cliente: {ClienteId}", clienteId);
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpGet("cliente/{clienteId}/ativa")]
    public async Task<ActionResult<ClienteRestricaoDto>> GetRestricaoAtivaByCliente(int clienteId)
    {
        try
        {
            var restricao = await _restricaoService.GetRestricaoAtivaByClienteAsync(clienteId);
            if (restricao == null)
                return NotFound("Cliente n�o possui restri��o ativa");

            return Ok(restricao);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar restri��o ativa por cliente: {ClienteId}", clienteId);
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpGet("motivo/{motivo}")]
    public async Task<ActionResult<IEnumerable<ClienteRestricaoListDto>>> GetByMotivo(MotivoRestricaoCliente motivo)
    {
        try
        {
            var restricoes = await _restricaoService.GetByMotivoAsync(motivo);
            return Ok(restricoes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar restri��es por motivo: {Motivo}", motivo);
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpGet("cliente/{clienteId}/verificar-restricao")]
    public async Task<ActionResult<bool>> VerificarRestricao(int clienteId)
    {
        try
        {
            var temRestricao = await _restricaoService.ClienteTemRestricaoAtivaAsync(clienteId);
            return Ok(new { temRestricaoAtiva = temRestricao });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao verificar restri��o do cliente: {ClienteId}", clienteId);
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Gerente")]
    public async Task<ActionResult<ClienteRestricaoDto>> Create([FromBody] ClienteRestricaoCreateDto restricaoDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var restricao = await _restricaoService.CreateAsync(restricaoDto);
            return CreatedAtAction(nameof(GetById), new { id = restricao.Id }, restricao);
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
            _logger.LogError(ex, "Erro ao criar restri��o de cliente");
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpPatch("{id}/remover")]
    [Authorize(Roles = "Admin,Gerente")]
    public async Task<ActionResult<ClienteRestricaoDto>> RemoverRestricao(int id, [FromBody] ClienteRestricaoRemoverDto removerDto)
    {
        if (id != removerDto.Id)
            return BadRequest("ID da restri��o n�o confere");

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var restricao = await _restricaoService.RemoverRestricaoAsync(removerDto);
            return Ok(restricao);
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
            _logger.LogError(ex, "Erro ao remover restri��o: {Id}", id);
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            await _restricaoService.DeleteAsync(id);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao deletar restri��o: {Id}", id);
            return StatusCode(500, "Erro interno do servidor");
        }
    }
}