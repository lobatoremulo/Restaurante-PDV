using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantePDV.Application.DTOs;
using RestaurantePDV.Application.Services;
using RestaurantePDV.Domain.Enums;

namespace RestaurantePDV.API.Controllers;

[ApiController]
[Authorize]
public class ProdutosController(IProdutoService produtoService, ILogger<ProdutosController> logger) : ControllerBase
{
    private readonly IProdutoService _produtoService = produtoService;
    private readonly ILogger<ProdutosController> _logger = logger;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProdutoListDto>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 50, [FromQuery] string? sortBy = null, [FromQuery] bool desc = false)
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
    public async Task<ActionResult<IEnumerable<ProdutoListDto>>> GetActive([FromQuery] int page = 1, [FromQuery] int pageSize = 50, [FromQuery] string? sortBy = null, [FromQuery] bool desc = false)
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
    public async Task<ActionResult<IEnumerable<ProdutoListDto>>> GetByTipo(TipoProduto tipo, [FromQuery] int page = 1, [FromQuery] int pageSize = 50, [FromQuery] string? sortBy = null, [FromQuery] bool desc = false)
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
    public async Task<ActionResult<IEnumerable<ProdutoListDto>>> GetByNome(string nome, [FromQuery] int page = 1, [FromQuery] int pageSize = 50, [FromQuery] string? sortBy = null, [FromQuery] bool desc = false)
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
    public async Task<ActionResult<IEnumerable<ProdutoListDto>>> GetComEstoqueBaixo([FromQuery] int page = 1, [FromQuery] int pageSize = 50, [FromQuery] string? sortBy = null, [FromQuery] bool desc = false)
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
    public async Task<ActionResult<IEnumerable<ProdutoListDto>>> GetDisponivelDelivery([FromQuery] int page = 1, [FromQuery] int pageSize = 50, [FromQuery] string? sortBy = null, [FromQuery] bool desc = false)
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
        try
        {
            var produto = await _produtoService.CreateAsync(produtoDto);
            return Ok(produto);
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
    public async Task<ActionResult<object>> VerificarCodigoBarras(string codigoBarras, [FromQuery] int? excludeId = null)
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
    [ResponseCache(Duration = 3600)]
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
            var todosProdutosTask = _produtoService.GetAllAsync();
            var produtosAtivosTask = _produtoService.GetActiveAsync();
            var produtosEstoqueBaixoTask = _produtoService.GetComEstoqueBaixoAsync();

            await Task.WhenAll(todosProdutosTask, produtosAtivosTask, produtosEstoqueBaixoTask);

            var todosProdutos = await todosProdutosTask;
            var produtosAtivos = await produtosAtivosTask;
            var produtosEstoqueBaixo = await produtosEstoqueBaixoTask;

            var estatisticas = new
            {
                TotalProdutos = todosProdutos.Count(),
                ProdutosAtivos = produtosAtivos.Count(),
                ProdutosInativos = todosProdutos.Count() - produtosAtivos.Count(),
                ProdutosEstoqueBaixo = produtosEstoqueBaixo.Count(),
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

    //private static IEnumerable<ProdutoListDto> ApplySorting(IEnumerable<ProdutoListDto> source, string? sortBy, bool desc)
    //{
    //    if (string.IsNullOrWhiteSpace(sortBy))
    //        return source.OrderBy(p => p.Nome);

    //    sortBy = sortBy.ToLowerInvariant();
    //    return (sortBy, desc) switch
    //    {
    //        ("nome", false) => source.OrderBy(p => p.Nome),
    //        ("nome", true) => source.OrderByDescending(p => p.Nome),
    //        ("preco", false) => source.OrderBy(p => p.PrecoVenda),
    //        ("preco", true) => source.OrderByDescending(p => p.PrecoVenda),
    //        ("tipo", false) => source.OrderBy(p => p.Tipo),
    //        ("tipo", true) => source.OrderByDescending(p => p.Tipo),
    //        ("estoque", false) => source.OrderBy(p => p.EstoqueAtual),
    //        ("estoque", true) => source.OrderByDescending(p => p.EstoqueAtual),
    //        _ => source.OrderBy(p => p.Nome)
    //    };
    //}

    //private static PagedResult<ProdutoListDto> Paginate(IEnumerable<ProdutoListDto> source, int page, int pageSize)
    //{
    //    page = page < 1 ? 1 : page;
    //    pageSize = pageSize < 1 ? 50 : pageSize;
    //    var total = source.Count();
    //    var items = source.Skip((page - 1) * pageSize).Take(pageSize);
    //    return new PagedResult<ProdutoListDto>
    //    {
    //        Items = items.ToList(),
    //        TotalItems = total,
    //        Page = page,
    //        PageSize = pageSize
    //    };
    //}
}