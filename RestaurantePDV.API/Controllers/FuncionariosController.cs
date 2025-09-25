using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantePDV.Application.DTOs;
using RestaurantePDV.Application.Services;
using RestaurantePDV.Domain.Enums;

namespace RestaurantePDV.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FuncionariosController : ControllerBase
{
    private readonly IFuncionarioService _funcionarioService;
    private readonly ILogger<FuncionariosController> _logger;

    public FuncionariosController(IFuncionarioService funcionarioService, ILogger<FuncionariosController> logger)
    {
        _funcionarioService = funcionarioService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<FuncionarioListDto>>> GetAll()
    {
        try
        {
            var funcionarios = await _funcionarioService.GetAllAsync();
            return Ok(funcionarios);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar funcionários");
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpGet("ativos")]
    public async Task<ActionResult<IEnumerable<FuncionarioListDto>>> GetActive()
    {
        try
        {
            var funcionarios = await _funcionarioService.GetActiveAsync();
            return Ok(funcionarios);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar funcionários ativos");
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<FuncionarioDto>> GetById(int id)
    {
        try
        {
            var funcionario = await _funcionarioService.GetByIdAsync(id);
            return Ok(funcionario);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar funcionário por ID: {Id}", id);
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpGet("cpf/{cpf}")]
    public async Task<ActionResult<FuncionarioDto>> GetByCpf(string cpf)
    {
        try
        {
            var funcionario = await _funcionarioService.GetByCpfAsync(cpf);
            if (funcionario == null)
                return NotFound("Funcionário não encontrado");

            return Ok(funcionario);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar funcionário por CPF: {Cpf}", cpf);
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpGet("cargo/{cargo}")]
    public async Task<ActionResult<IEnumerable<FuncionarioListDto>>> GetByCargo(string cargo)
    {
        try
        {
            var funcionarios = await _funcionarioService.GetByCargoAsync(cargo);
            return Ok(funcionarios);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar funcionários por cargo: {Cargo}", cargo);
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpGet("setor/{setor}")]
    public async Task<ActionResult<IEnumerable<FuncionarioListDto>>> GetBySetor(string setor)
    {
        try
        {
            var funcionarios = await _funcionarioService.GetBySetorAsync(setor);
            return Ok(funcionarios);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar funcionários por setor: {Setor}", setor);
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpGet("nivel-acesso/{nivelAcesso}")]
    public async Task<ActionResult<IEnumerable<FuncionarioListDto>>> GetByNivelAcesso(NivelAcesso nivelAcesso)
    {
        try
        {
            var funcionarios = await _funcionarioService.GetByNivelAcessoAsync(nivelAcesso);
            return Ok(funcionarios);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar funcionários por nível de acesso: {NivelAcesso}", nivelAcesso);
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpGet("status/{status}")]
    public async Task<ActionResult<IEnumerable<FuncionarioListDto>>> GetByStatus(StatusFuncionario status)
    {
        try
        {
            var funcionarios = await _funcionarioService.GetByStatusAsync(status);
            return Ok(funcionarios);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar funcionários por status: {Status}", status);
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Gerente")]
    public async Task<ActionResult<FuncionarioDto>> Create([FromBody] FuncionarioCreateDto funcionarioDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var funcionario = await _funcionarioService.CreateAsync(funcionarioDto);
            return CreatedAtAction(nameof(GetById), new { id = funcionario.Id }, funcionario);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar funcionário");
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Gerente")]
    public async Task<ActionResult<FuncionarioDto>> Update(int id, [FromBody] FuncionarioUpdateDto funcionarioDto)
    {
        if (id != funcionarioDto.Id)
            return BadRequest("ID do funcionário não confere");

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var funcionario = await _funcionarioService.UpdateAsync(funcionarioDto);
            return Ok(funcionario);
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
            _logger.LogError(ex, "Erro ao atualizar funcionário: {Id}", id);
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpPatch("{id}/inativar")]
    [Authorize(Roles = "Admin,Gerente")]
    public async Task<ActionResult<FuncionarioDto>> Inativar(int id)
    {
        try
        {
            var funcionario = await _funcionarioService.InativarAsync(id);
            return Ok(funcionario);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao inativar funcionário: {Id}", id);
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpPatch("{id}/ativar")]
    [Authorize(Roles = "Admin,Gerente")]
    public async Task<ActionResult<FuncionarioDto>> Ativar(int id)
    {
        try
        {
            var funcionario = await _funcionarioService.AtivarAsync(id);
            return Ok(funcionario);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao ativar funcionário: {Id}", id);
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpPatch("{id}/demitir")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<FuncionarioDto>> Demitir(int id, [FromBody] DateTime dataDemissao)
    {
        try
        {
            var funcionario = await _funcionarioService.DemitirAsync(id, dataDemissao);
            return Ok(funcionario);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao demitir funcionário: {Id}", id);
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            await _funcionarioService.DeleteAsync(id);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao deletar funcionário: {Id}", id);
            return StatusCode(500, "Erro interno do servidor");
        }
    }
}