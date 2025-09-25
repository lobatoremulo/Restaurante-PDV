using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantePDV.Application.DTOs;
using RestaurantePDV.Application.Services;
using RestaurantePDV.Domain.Enums;

namespace RestaurantePDV.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProdutosController(IProdutoService produtoService, ILogger<ProdutosController> logger) : ControllerBase
{
    private readonly IProdutoService _produtoService = produtoService;
    private readonly ILogger<ProdutosController> _logger = logger;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProdutoListDto>>> GetAll()
    {
        try
        {
            var produtos = await _produtoService.GetAllAsync();
            return Ok(produtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar produtos");
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpGet("ativos")]
    public async Task<ActionResult<IEnumerable<ProdutoListDto>>> GetActive()
    {
        try
        {
            var produtos = await _produtoService.GetActiveAsync();
            return Ok(produtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar produtos ativos");
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProdutoDto>> GetById(int id)
    {
        try
        {
            var produto = await _produtoService.GetByIdAsync(id);
            return Ok(produto);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar produto por ID: {Id}", id);
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpGet("codigo-barras/{codigoBarras}")]
    public async Task<ActionResult<ProdutoDto>> GetByCodigoBarras(string codigoBarras)
    {
        try
        {
            var produto = await _produtoService.GetByCodigoBarrasAsync(codigoBarras);
            if (produto == null)
                return NotFound("Produto não encontrado");

            return Ok(produto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar produto por código de barras: {CodigoBarras}", codigoBarras);
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpGet("tipo/{tipo}")]
    public async Task<ActionResult<IEnumerable<ProdutoListDto>>> GetByTipo(TipoProduto tipo)
    {
        try
        {
            var produtos = await _produtoService.GetByTipoAsync(tipo);
            return Ok(produtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar produtos por tipo: {Tipo}", tipo);
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpGet("buscar/{nome}")]
    public async Task<ActionResult<IEnumerable<ProdutoListDto>>> GetByNome(string nome)
    {
        try
        {
            var produtos = await _produtoService.GetByNomeAsync(nome);
            return Ok(produtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar produtos por nome: {Nome}", nome);
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpGet("estoque-baixo")]
    public async Task<ActionResult<IEnumerable<ProdutoListDto>>> GetComEstoqueBaixo()
    {
        try
        {
            var produtos = await _produtoService.GetComEstoqueBaixoAsync();
            return Ok(produtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar produtos com estoque baixo");
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpGet("delivery")]
    public async Task<ActionResult<IEnumerable<ProdutoListDto>>> GetDisponivelDelivery()
    {
        try
        {
            var produtos = await _produtoService.GetDisponivelDeliveryAsync();
            return Ok(produtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar produtos disponíveis para delivery");
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Gerente")]
    public async Task<ActionResult<ProdutoDto>> Create([FromBody] ProdutoCreateDto produtoDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var produto = await _produtoService.CreateAsync(produtoDto);
            return CreatedAtAction(nameof(GetById), new { id = produto.Id }, produto);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar produto");
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Gerente")]
    public async Task<ActionResult<ProdutoDto>> Update(int id, [FromBody] ProdutoUpdateDto produtoDto)
    {
        if (id != produtoDto.Id)
            return BadRequest("ID do produto não confere");

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var produto = await _produtoService.UpdateAsync(produtoDto);
            return Ok(produto);
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
            _logger.LogError(ex, "Erro ao atualizar produto: {Id}", id);
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpPatch("{id}/inativar")]
    [Authorize(Roles = "Admin,Gerente")]
    public async Task<ActionResult<ProdutoDto>> Inativar(int id)
    {
        try
        {
            var produto = await _produtoService.InativarAsync(id);
            return Ok(produto);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao inativar produto: {Id}", id);
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpPatch("{id}/ativar")]
    [Authorize(Roles = "Admin,Gerente")]
    public async Task<ActionResult<ProdutoDto>> Ativar(int id)
    {
        try
        {
            var produto = await _produtoService.AtivarAsync(id);
            return Ok(produto);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao ativar produto: {Id}", id);
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpPatch("estoque")]
    [Authorize(Roles = "Admin,Gerente,UsuarioComum")]
    public async Task<ActionResult<ProdutoDto>> AtualizarEstoque([FromBody] ProdutoEstoqueDto estoqueDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var produto = await _produtoService.AtualizarEstoqueAsync(estoqueDto);
            return Ok(produto);
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
            _logger.LogError(ex, "Erro ao atualizar estoque do produto: {ProdutoId}", estoqueDto.ProdutoId);
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            await _produtoService.DeleteAsync(id);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao deletar produto: {Id}", id);
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpGet("verificar-codigo-barras/{codigoBarras}")]
    public async Task<ActionResult<bool>> VerificarCodigoBarras(string codigoBarras, [FromQuery] int? excludeId = null)
    {
        try
        {
            var exists = await _produtoService.CodigoBarrasExistsAsync(codigoBarras, excludeId);
            return Ok(new { existe = exists });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao verificar código de barras: {CodigoBarras}", codigoBarras);
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpGet("tipos")]
    public ActionResult<object> GetTipos()
    {
        try
        {
            var tipos = Enum.GetValues<TipoProduto>()
                .Select(t => new { value = (int)t, name = t.ToString() })
                .ToList();
            
            return Ok(tipos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar tipos de produto");
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpGet("estatisticas")]
    [Authorize(Roles = "Admin,Gerente")]
    public async Task<ActionResult<object>> GetEstatisticas()
    {
        try
        {
            var todosProdutos = await _produtoService.GetAllAsync();
            var produtosAtivos = await _produtoService.GetActiveAsync();
            var produtosEstoqueBaixo = await _produtoService.GetComEstoqueBaixoAsync();
            var produtosDelivery = await _produtoService.GetDisponivelDeliveryAsync();

            var estatisticas = new
            {
                TotalProdutos = todosProdutos.Count(),
                ProdutosAtivos = produtosAtivos.Count(),
                ProdutosInativos = todosProdutos.Count() - produtosAtivos.Count(),
                ProdutosEstoqueBaixo = produtosEstoqueBaixo.Count(),
                ProdutosDelivery = produtosDelivery.Count(),
                PorTipo = todosProdutos.GroupBy(p => p.Tipo)
                    .Select(g => new { Tipo = g.Key.ToString(), Quantidade = g.Count() })
                    .ToList()
            };

            return Ok(estatisticas);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar estatísticas de produtos");
            return StatusCode(500, "Erro interno do servidor");
        }
    }
}