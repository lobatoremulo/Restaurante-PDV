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

    [HttpGet("comanda/{comandaId}/preparar-pagamento")]
    public async Task<ActionResult<ComandaParaPagamentoDto>> PrepararPagamentoComanda(int comandaId)
    {
        try
        {
            var comanda = await _vendaService.PrepararPagamentoComandaAsync(comandaId);
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

    [HttpPost("pagar-comanda")]
    public async Task<ActionResult<VendaComandaDto>> PagarComanda([FromBody] PagamentoComandaDto pagamentoDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var venda = await _vendaService.ProcessarPagamentoComandaAsync(pagamentoDto);
            return CreatedAtAction(nameof(GetVendaComanda), new { id = venda.Id }, venda);
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

    [HttpGet("venda-comanda/{id}")]
    public async Task<ActionResult<VendaComandaDto>> GetVendaComanda(int id)
    {
        try
        {
            var venda = await _vendaService.GetVendaComandaAsync(id);
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

    [HttpPost("validar-pagamento")]
    public async Task<ActionResult<object>> ValidarPagamento([FromBody] PagamentoComandaDto pagamentoDto)
    {
        try
        {
            var validacao = await _vendaService.ValidarPagamentoComandaAsync(pagamentoDto);
            return Ok(validacao);
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

    [HttpGet("comandas-pendentes")]
    public async Task<ActionResult<IEnumerable<ComandaParaPagamentoDto>>> GetComandasPendentes()
    {
        try
        {
            var comandas = await _vendaService.GetComandasPendentesAsync();
            return Ok(comandas);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPost("reprocessar-pagamento/{vendaId}")]
    public async Task<ActionResult<VendaComandaDto>> ReprocessarPagamento(int vendaId, [FromBody] PagamentoComandaDto pagamentoDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var venda = await _vendaService.ReprocessarPagamentoAsync(vendaId, pagamentoDto);
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

    [HttpGet("relatorio-vendas")]
    public async Task<ActionResult<object>> GetRelatorioVendas([FromQuery] DateTime dataInicio, [FromQuery] DateTime dataFim)
    {
        try
        {
            var relatorio = await _vendaService.GetRelatorioVendasAsync(dataInicio, dataFim);
            return Ok(relatorio);
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