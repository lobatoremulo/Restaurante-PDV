using Microsoft.AspNetCore.Mvc;
using RestaurantePDV.Application.DTOs;
using RestaurantePDV.Application.Services;
using RestaurantePDV.Domain.Enums;

namespace RestaurantePDV.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VendasController : ControllerBase
{
    private readonly IVendaService _vendaService;

    public VendasController(IVendaService vendaService)
    {
        _vendaService = vendaService;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<VendaDto>> GetById(int id)
    {
        try
        {
            var venda = await _vendaService.GetByIdAsync(id);
            return Ok(venda);
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

    [HttpGet("numero/{numeroVenda}")]
    public async Task<ActionResult<VendaDto>> GetByNumero(string numeroVenda)
    {
        try
        {
            var venda = await _vendaService.GetByNumeroAsync(numeroVenda);
            if (venda == null)
                return NotFound();

            return Ok(venda);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("periodo")]
    public async Task<ActionResult<IEnumerable<VendaDto>>> GetByPeriodo([FromQuery] DateTime dataInicio, [FromQuery] DateTime dataFim)
    {
        try
        {
            var vendas = await _vendaService.GetByPeriodoAsync(dataInicio, dataFim);
            return Ok(vendas);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("total-periodo")]
    public async Task<ActionResult<decimal>> GetTotalVendasPeriodo([FromQuery] DateTime dataInicio, [FromQuery] DateTime dataFim)
    {
        try
        {
            var total = await _vendaService.GetTotalVendasPeriodoAsync(dataInicio, dataFim);
            return Ok(total);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPost]
    public async Task<ActionResult<VendaDto>> CriarVenda([FromBody] VendaCreateDto vendaDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var venda = await _vendaService.CriarVendaAsync(vendaDto);
            return CreatedAtAction(nameof(GetById), new { id = venda.Id }, venda);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPut("{id}/finalizar")]
    public async Task<ActionResult<VendaDto>> FinalizarVenda(int id, [FromBody] FinalizarVendaRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var venda = await _vendaService.FinalizarVendaAsync(id, request.FormaPagamento, request.ValorPago);
            return Ok(venda);
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
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPut("{id}/cancelar")]
    public async Task<ActionResult<VendaDto>> CancelarVenda(int id)
    {
        try
        {
            var venda = await _vendaService.CancelarVendaAsync(id);
            return Ok(venda);
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
}

public class FinalizarVendaRequest
{
    public FormaPagamento FormaPagamento { get; set; }
    public decimal ValorPago { get; set; }
}