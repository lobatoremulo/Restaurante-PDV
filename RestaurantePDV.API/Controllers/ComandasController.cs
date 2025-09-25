using Microsoft.AspNetCore.Mvc;
using RestaurantePDV.Application.DTOs;
using RestaurantePDV.Application.Services;

namespace RestaurantePDV.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ComandasController : ControllerBase
{
    private readonly IComandaService _comandaService;

    public ComandasController(IComandaService comandaService)
    {
        _comandaService = comandaService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ComandaDto>>> GetAtivasAsync()
    {
        try
        {
            var comandas = await _comandaService.GetAtivasAsync();
            return Ok(comandas);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ComandaDto>> GetById(int id)
    {
        try
        {
            var comanda = await _comandaService.GetByIdAsync(id);
            return Ok(comanda);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("numero/{numeroComanda}")]
    public async Task<ActionResult<ComandaDto>> GetByNumero(string numeroComanda)
    {
        try
        {
            var comanda = await _comandaService.GetByNumeroAsync(numeroComanda);
            if (comanda == null)
                return NotFound();

            return Ok(comanda);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("periodo")]
    public async Task<ActionResult<IEnumerable<ComandaDto>>> GetByPeriodo([FromQuery] DateTime dataInicio, [FromQuery] DateTime dataFim)
    {
        try
        {
            var comandas = await _comandaService.GetByPeriodoAsync(dataInicio, dataFim);
            return Ok(comandas);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPost]
    public async Task<ActionResult<ComandaDto>> AbrirComanda([FromBody] ComandaCreateDto comandaDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var comanda = await _comandaService.AbrirComandaAsync(comandaDto);
            return CreatedAtAction(nameof(GetById), new { id = comanda.Id }, comanda);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPut("{id}/fechar")]
    public async Task<ActionResult<ComandaDto>> FecharComanda(int id)
    {
        try
        {
            var comanda = await _comandaService.FecharComandaAsync(id);
            return Ok(comanda);
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
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPut("{id}/cancelar")]
    public async Task<ActionResult<ComandaDto>> CancelarComanda(int id)
    {
        try
        {
            var comanda = await _comandaService.CancelarComandaAsync(id);
            return Ok(comanda);
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
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPost("{id}/itens")]
    public async Task<ActionResult<ComandaDto>> AdicionarItem(int id, [FromBody] ItemComandaCreateDto itemDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            itemDto.ComandaId = id;
            var comanda = await _comandaService.AdicionarItemAsync(id, itemDto);
            return Ok(comanda);
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
            return StatusCode(500, ex.Message);
        }
    }

    [HttpDelete("{comandaId}/itens/{itemId}")]
    public async Task<ActionResult<ComandaDto>> RemoverItem(int comandaId, int itemId)
    {
        try
        {
            var comanda = await _comandaService.RemoverItemAsync(comandaId, itemId);
            return Ok(comanda);
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
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPut("itens/{itemId}/preparado")]
    public async Task<ActionResult<ComandaDto>> MarcarItemPreparado(int itemId)
    {
        try
        {
            var comanda = await _comandaService.MarcarItemPreparadoAsync(itemId);
            return Ok(comanda);
        }
        catch (NotImplementedException)
        {
            return StatusCode(501, "Funcionalidade não implementada");
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPut("itens/{itemId}/entregue")]
    public async Task<ActionResult<ComandaDto>> MarcarItemEntregue(int itemId)
    {
        try
        {
            var comanda = await _comandaService.MarcarItemEntregueAsync(itemId);
            return Ok(comanda);
        }
        catch (NotImplementedException)
        {
            return StatusCode(501, "Funcionalidade não implementada");
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}