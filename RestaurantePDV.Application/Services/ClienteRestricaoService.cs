using RestaurantePDV.Application.DTOs;
using RestaurantePDV.Application.Interfaces;
using RestaurantePDV.Domain.Entities;
using RestaurantePDV.Domain.Enums;

namespace RestaurantePDV.Application.Services;

public interface IClienteRestricaoService
{
    Task<ClienteRestricaoDto> CreateAsync(ClienteRestricaoCreateDto restricaoDto);
    Task<ClienteRestricaoDto> RemoverRestricaoAsync(ClienteRestricaoRemoverDto removerDto);
    Task<ClienteRestricaoDto> GetByIdAsync(int id);
    Task<IEnumerable<ClienteRestricaoListDto>> GetAllAsync();
    Task<IEnumerable<ClienteRestricaoListDto>> GetByClienteAsync(int clienteId);
    Task<IEnumerable<ClienteRestricaoListDto>> GetRestricoesAtivasAsync();
    Task<IEnumerable<ClienteRestricaoListDto>> GetByMotivoAsync(MotivoRestricaoCliente motivo);
    Task<ClienteRestricaoDto?> GetRestricaoAtivaByClienteAsync(int clienteId);
    Task<IEnumerable<ClienteComRestricaoDto>> GetClientesComRestricaoAsync();
    Task<bool> ClienteTemRestricaoAtivaAsync(int clienteId);
    Task DeleteAsync(int id);
}

public class ClienteRestricaoService : IClienteRestricaoService
{
    private readonly IClienteRestricaoRepository _restricaoRepository;
    private readonly IClienteRepository _clienteRepository;
    private readonly IFuncionarioRepository _funcionarioRepository;

    public ClienteRestricaoService(
        IClienteRestricaoRepository restricaoRepository,
        IClienteRepository clienteRepository,
        IFuncionarioRepository funcionarioRepository)
    {
        _restricaoRepository = restricaoRepository;
        _clienteRepository = clienteRepository;
        _funcionarioRepository = funcionarioRepository;
    }

    public async Task<ClienteRestricaoDto> CreateAsync(ClienteRestricaoCreateDto restricaoDto)
    {
        // Validar se cliente existe
        var cliente = await _clienteRepository.GetByIdAsync(restricaoDto.ClienteId);
        if (cliente == null)
            throw new ArgumentException("Cliente não encontrado");

        // Validar se funcionário responsável existe
        var responsavel = await _funcionarioRepository.GetByIdAsync(restricaoDto.ResponsavelInclusaoId);
        if (responsavel == null)
            throw new ArgumentException("Funcionário responsável não encontrado");

        // Verificar se cliente já possui restrição ativa
        if (await ClienteTemRestricaoAtivaAsync(restricaoDto.ClienteId))
            throw new InvalidOperationException("Cliente já possui restrição ativa");

        var restricao = new ClienteRestricao
        {
            ClienteId = restricaoDto.ClienteId,
            Motivo = restricaoDto.Motivo,
            Descricao = restricaoDto.Descricao,
            DataInclusao = DateTime.UtcNow,
            ResponsavelInclusaoId = restricaoDto.ResponsavelInclusaoId,
            Ativa = true
        };

        var restricaoCriada = await _restricaoRepository.AddAsync(restricao);
        return await MapToDtoAsync(restricaoCriada);
    }

    public async Task<ClienteRestricaoDto> RemoverRestricaoAsync(ClienteRestricaoRemoverDto removerDto)
    {
        var restricao = await _restricaoRepository.GetByIdAsync(removerDto.Id);
        if (restricao == null)
            throw new ArgumentException("Restrição não encontrada");

        if (!restricao.Ativa)
            throw new InvalidOperationException("Restrição já foi removida");

        // Validar se funcionário responsável pela remoção existe
        var responsavel = await _funcionarioRepository.GetByIdAsync(removerDto.ResponsavelRemocaoId);
        if (responsavel == null)
            throw new ArgumentException("Funcionário responsável pela remoção não encontrado");

        restricao.Ativa = false;
        restricao.DataRemocao = DateTime.UtcNow;
        restricao.ResponsavelRemocaoId = removerDto.ResponsavelRemocaoId;
        restricao.ObservacoesRemocao = removerDto.ObservacoesRemocao;
        restricao.AtualizadoEm = DateTime.UtcNow;

        var restricaoAtualizada = await _restricaoRepository.UpdateAsync(restricao);
        return await MapToDtoAsync(restricaoAtualizada);
    }

    public async Task<ClienteRestricaoDto> GetByIdAsync(int id)
    {
        var restricao = await _restricaoRepository.GetByIdAsync(id);
        if (restricao == null)
            throw new ArgumentException("Restrição não encontrada");

        return await MapToDtoAsync(restricao);
    }

    public async Task<IEnumerable<ClienteRestricaoListDto>> GetAllAsync()
    {
        var restricoes = await _restricaoRepository.GetComDetalhesAsync();
        return await MapToListDtoAsync(restricoes);
    }

    public async Task<IEnumerable<ClienteRestricaoListDto>> GetByClienteAsync(int clienteId)
    {
        var restricoes = await _restricaoRepository.GetByClienteAsync(clienteId);
        return await MapToListDtoAsync(restricoes);
    }

    public async Task<IEnumerable<ClienteRestricaoListDto>> GetRestricoesAtivasAsync()
    {
        var restricoes = await _restricaoRepository.GetRestricoesAtivasAsync();
        return await MapToListDtoAsync(restricoes);
    }

    public async Task<IEnumerable<ClienteRestricaoListDto>> GetByMotivoAsync(MotivoRestricaoCliente motivo)
    {
        var restricoes = await _restricaoRepository.GetByMotivoAsync(motivo);
        return await MapToListDtoAsync(restricoes);
    }

    public async Task<ClienteRestricaoDto?> GetRestricaoAtivaByClienteAsync(int clienteId)
    {
        var restricao = await _restricaoRepository.GetRestricaoAtivaByClienteAsync(clienteId);
        return restricao != null ? await MapToDtoAsync(restricao) : null;
    }

    public async Task<IEnumerable<ClienteComRestricaoDto>> GetClientesComRestricaoAsync()
    {
        var restricoes = await _restricaoRepository.GetRestricoesAtivasAsync();
        var clientesComRestricao = new List<ClienteComRestricaoDto>();

        var clientesAgrupados = restricoes.GroupBy(r => r.ClienteId);

        foreach (var grupo in clientesAgrupados)
        {
            var cliente = await _clienteRepository.GetByIdAsync(grupo.Key);
            if (cliente != null)
            {
                var clienteDto = new ClienteComRestricaoDto
                {
                    Id = cliente.Id,
                    Nome = cliente.Nome,
                    CpfCnpj = cliente.CpfCnpj,
                    Telefone = cliente.Telefone,
                    Email = cliente.Email,
                    TemRestricaoAtiva = true,
                    Restricoes = new List<ClienteRestricaoDto>()
                };

                foreach (var restricao in grupo)
                {
                    clienteDto.Restricoes.Add(await MapToDtoAsync(restricao));
                }

                clientesComRestricao.Add(clienteDto);
            }
        }

        return clientesComRestricao;
    }

    public async Task<bool> ClienteTemRestricaoAtivaAsync(int clienteId)
    {
        var restricao = await _restricaoRepository.GetRestricaoAtivaByClienteAsync(clienteId);
        return restricao != null;
    }

    public async Task DeleteAsync(int id)
    {
        var restricao = await _restricaoRepository.GetByIdAsync(id);
        if (restricao == null)
            throw new ArgumentException("Restrição não encontrada");

        await _restricaoRepository.DeleteAsync(id);
    }

    private async Task<ClienteRestricaoDto> MapToDtoAsync(ClienteRestricao restricao)
    {
        var cliente = await _clienteRepository.GetByIdAsync(restricao.ClienteId);
        var responsavelInclusao = await _funcionarioRepository.GetByIdAsync(restricao.ResponsavelInclusaoId);
        var responsavelRemocao = restricao.ResponsavelRemocaoId.HasValue 
            ? await _funcionarioRepository.GetByIdAsync(restricao.ResponsavelRemocaoId.Value) 
            : null;

        return new ClienteRestricaoDto
        {
            Id = restricao.Id,
            ClienteId = restricao.ClienteId,
            ClienteNome = cliente?.Nome ?? "",
            Motivo = restricao.Motivo,
            Descricao = restricao.Descricao,
            DataInclusao = restricao.DataInclusao,
            DataRemocao = restricao.DataRemocao,
            ResponsavelInclusaoId = restricao.ResponsavelInclusaoId,
            ResponsavelInclusaoNome = responsavelInclusao?.Nome ?? "",
            ResponsavelRemocaoId = restricao.ResponsavelRemocaoId,
            ResponsavelRemocaoNome = responsavelRemocao?.Nome,
            ObservacoesRemocao = restricao.ObservacoesRemocao,
            Ativa = restricao.Ativa,
            CriadoEm = restricao.CriadoEm,
            AtualizadoEm = restricao.AtualizadoEm
        };
    }

    private async Task<IEnumerable<ClienteRestricaoListDto>> MapToListDtoAsync(IEnumerable<ClienteRestricao> restricoes)
    {
        var result = new List<ClienteRestricaoListDto>();

        foreach (var restricao in restricoes)
        {
            var cliente = await _clienteRepository.GetByIdAsync(restricao.ClienteId);
            var responsavelInclusao = await _funcionarioRepository.GetByIdAsync(restricao.ResponsavelInclusaoId);

            result.Add(new ClienteRestricaoListDto
            {
                Id = restricao.Id,
                ClienteId = restricao.ClienteId,
                ClienteNome = cliente?.Nome ?? "",
                Motivo = restricao.Motivo,
                Descricao = restricao.Descricao,
                DataInclusao = restricao.DataInclusao,
                ResponsavelInclusaoNome = responsavelInclusao?.Nome ?? "",
                Ativa = restricao.Ativa
            });
        }

        return result;
    }
}