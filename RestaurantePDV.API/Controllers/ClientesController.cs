using Microsoft.AspNetCore.Mvc;
using RestaurantePDV.Application.DTOs;
using RestaurantePDV.Application.Interfaces;

namespace RestaurantePDV.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientesController : ControllerBase
{
    private readonly IClienteRepository _clienteRepository;

    public ClientesController(IClienteRepository clienteRepository)
    {
        _clienteRepository = clienteRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ClienteDto>>> GetAll()
    {
        var clientes = await _clienteRepository.GetActiveAsync();
        var clienteDtos = clientes.Select(MapToDto);
        return Ok(clienteDtos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ClienteDto>> GetById(int id)
    {
        var cliente = await _clienteRepository.GetByIdAsync(id);
        if (cliente == null)
            return NotFound();

        return Ok(MapToDto(cliente));
    }

    [HttpGet("cpf-cnpj/{cpfCnpj}")]
    public async Task<ActionResult<ClienteDto>> GetByCpfCnpj(string cpfCnpj)
    {
        var cliente = await _clienteRepository.GetByCpfCnpjAsync(cpfCnpj);
        if (cliente == null)
            return NotFound();

        return Ok(MapToDto(cliente));
    }

    [HttpGet("search/{nome}")]
    public async Task<ActionResult<IEnumerable<ClienteDto>>> SearchByNome(string nome)
    {
        var clientes = await _clienteRepository.GetByNomeAsync(nome);
        var clienteDtos = clientes.Select(MapToDto);
        return Ok(clienteDtos);
    }

    [HttpGet("com-limite-credito")]
    public async Task<ActionResult<IEnumerable<ClienteDto>>> GetComLimiteCredito()
    {
        var clientes = await _clienteRepository.GetComLimiteCreditoAsync();
        var clienteDtos = clientes.Select(MapToDto);
        return Ok(clienteDtos);
    }

    [HttpPost]
    public async Task<ActionResult<ClienteDto>> Create([FromBody] ClienteCreateDto clienteDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Verificar se CPF/CNPJ já existe
        if (!string.IsNullOrEmpty(clienteDto.CpfCnpj))
        {
            var clienteExistente = await _clienteRepository.GetByCpfCnpjAsync(clienteDto.CpfCnpj);
            if (clienteExistente != null)
                return Conflict("CPF/CNPJ já cadastrado");
        }

        var cliente = MapToEntity(clienteDto);
        var clienteCriado = await _clienteRepository.AddAsync(cliente);

        return CreatedAtAction(nameof(GetById), new { id = clienteCriado.Id }, MapToDto(clienteCriado));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ClienteDto>> Update(int id, [FromBody] ClienteUpdateDto clienteDto)
    {
        if (id != clienteDto.Id)
            return BadRequest();

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var clienteExistente = await _clienteRepository.GetByIdAsync(id);
        if (clienteExistente == null)
            return NotFound();

        // Verificar se CPF/CNPJ já existe em outro cliente
        if (!string.IsNullOrEmpty(clienteDto.CpfCnpj))
        {
            var outroCliente = await _clienteRepository.GetByCpfCnpjAsync(clienteDto.CpfCnpj);
            if (outroCliente != null && outroCliente.Id != id)
                return Conflict("CPF/CNPJ já cadastrado para outro cliente");
        }

        // Atualizar propriedades
        clienteExistente.Nome = clienteDto.Nome;
        clienteExistente.CpfCnpj = clienteDto.CpfCnpj;
        clienteExistente.Telefone = clienteDto.Telefone;
        clienteExistente.Email = clienteDto.Email;
        clienteExistente.Endereco = clienteDto.Endereco;
        clienteExistente.Cidade = clienteDto.Cidade;
        clienteExistente.Estado = clienteDto.Estado;
        clienteExistente.Cep = clienteDto.Cep;
        clienteExistente.DataNascimento = clienteDto.DataNascimento;
        clienteExistente.LimiteCredito = clienteDto.LimiteCredito;
        clienteExistente.Observacoes = clienteDto.Observacoes;

        var clienteAtualizado = await _clienteRepository.UpdateAsync(clienteExistente);
        return Ok(MapToDto(clienteAtualizado));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var cliente = await _clienteRepository.GetByIdAsync(id);
        if (cliente == null)
            return NotFound();

        await _clienteRepository.DeleteAsync(id);
        return NoContent();
    }

    private ClienteDto MapToDto(Domain.Entities.Cliente cliente)
    {
        return new ClienteDto
        {
            Id = cliente.Id,
            Nome = cliente.Nome,
            CpfCnpj = cliente.CpfCnpj,
            Telefone = cliente.Telefone,
            Email = cliente.Email,
            Endereco = cliente.Endereco,
            Cidade = cliente.Cidade,
            Estado = cliente.Estado,
            Cep = cliente.Cep,
            DataNascimento = cliente.DataNascimento,
            LimiteCredito = cliente.LimiteCredito,
            Observacoes = cliente.Observacoes,
            CriadoEm = cliente.CriadoEm,
            AtualizadoEm = cliente.AtualizadoEm,
            Ativo = cliente.Ativo
        };
    }

    private Domain.Entities.Cliente MapToEntity(ClienteCreateDto clienteDto)
    {
        return new Domain.Entities.Cliente
        {
            Nome = clienteDto.Nome,
            CpfCnpj = clienteDto.CpfCnpj,
            Telefone = clienteDto.Telefone,
            Email = clienteDto.Email,
            Endereco = clienteDto.Endereco,
            Cidade = clienteDto.Cidade,
            Estado = clienteDto.Estado,
            Cep = clienteDto.Cep,
            DataNascimento = clienteDto.DataNascimento,
            LimiteCredito = clienteDto.LimiteCredito,
            Observacoes = clienteDto.Observacoes
        };
    }
}