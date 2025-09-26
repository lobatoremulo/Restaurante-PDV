using RestaurantePDV.Application.DTOs;
using RestaurantePDV.Application.Interfaces;
using RestaurantePDV.Domain.Entities;
using RestaurantePDV.Domain.Enums;

namespace RestaurantePDV.Application.Services;

public interface IProdutoService
{
    Task<ProdutoDto> CreateAsync(ProdutoCreateDto produtoDto);
    Task<ProdutoDto> UpdateAsync(ProdutoUpdateDto produtoDto);
    Task<ProdutoDto> GetByIdAsync(int id);
    Task<ProdutoDto?> GetByCodigoBarrasAsync(string codigoBarras);
    Task<IEnumerable<ProdutoListDto>> GetAllAsync();
    Task<IEnumerable<ProdutoListDto>> GetActiveAsync();
    Task<IEnumerable<ProdutoListDto>> GetByTipoAsync(TipoProduto tipo);
    Task<IEnumerable<ProdutoListDto>> GetByNomeAsync(string nome);
    Task<IEnumerable<ProdutoListDto>> GetComEstoqueBaixoAsync();
    Task<IEnumerable<ProdutoListDto>> GetDisponivelDeliveryAsync();
    Task<ProdutoDto> InativarAsync(int id);
    Task<ProdutoDto> AtivarAsync(int id);
    Task<ProdutoDto> AtualizarEstoqueAsync(ProdutoEstoqueDto estoqueDto);
    Task DeleteAsync(int id);
    Task<bool> CodigoBarrasExistsAsync(string codigoBarras, int? excludeId = null);
}

public class ProdutoService : IProdutoService
{
    private readonly IProdutoRepository _produtoRepository;

    public ProdutoService(IProdutoRepository produtoRepository)
    {
        _produtoRepository = produtoRepository;
    }

    public async Task<ProdutoDto> CreateAsync(ProdutoCreateDto produtoDto)
    {
        // Validar código de barras único (se informado)
        if (!string.IsNullOrEmpty(produtoDto.CodigoBarras) && 
            await CodigoBarrasExistsAsync(produtoDto.CodigoBarras))
            throw new InvalidOperationException("Código de barras já cadastrado");

        var produto = new Produto
        {
            Nome = produtoDto.Nome,
            Descricao = produtoDto.Descricao,
            Tipo = produtoDto.Tipo,
            PrecoVenda = produtoDto.PrecoVenda,
            PrecoCusto = produtoDto.PrecoCusto,
            CodigoBarras = produtoDto.CodigoBarras,
            Unidade = produtoDto.Unidade,
            ControlaNaoEstoque = produtoDto.ControlaNaoEstoque,
            EstoqueMinimo = produtoDto.EstoqueMinimo,
            EstoqueAtual = produtoDto.EstoqueAtual,
            CaminhoFoto = produtoDto.CaminhoFoto,
            Ingredientes = produtoDto.Ingredientes,
            TempoPreparoMinutos = produtoDto.TempoPreparoMinutos,
            DisponivelDelivery = produtoDto.DisponivelDelivery
        };

        var produtoCriado = await _produtoRepository.AddAsync(produto);
        return MapToDto(produtoCriado);
    }

    public async Task<ProdutoDto> UpdateAsync(ProdutoUpdateDto produtoDto)
    {
        var produto = await _produtoRepository.GetByIdAsync(produtoDto.Id);
        if (produto == null)
            throw new ArgumentException("Produto não encontrado");

        // Validar código de barras único (se informado e diferente do atual)
        if (!string.IsNullOrEmpty(produtoDto.CodigoBarras) && 
            await CodigoBarrasExistsAsync(produtoDto.CodigoBarras, produtoDto.Id))
            throw new InvalidOperationException("Código de barras já cadastrado para outro produto");

        produto.Nome = produtoDto.Nome;
        produto.Descricao = produtoDto.Descricao;
        produto.Tipo = produtoDto.Tipo;
        produto.PrecoVenda = produtoDto.PrecoVenda;
        produto.PrecoCusto = produtoDto.PrecoCusto;
        produto.CodigoBarras = produtoDto.CodigoBarras;
        produto.Unidade = produtoDto.Unidade;
        produto.ControlaNaoEstoque = produtoDto.ControlaNaoEstoque;
        produto.EstoqueMinimo = produtoDto.EstoqueMinimo;
        produto.EstoqueAtual = produtoDto.EstoqueAtual;
        produto.CaminhoFoto = produtoDto.CaminhoFoto;
        produto.Ingredientes = produtoDto.Ingredientes;
        produto.TempoPreparoMinutos = produtoDto.TempoPreparoMinutos;
        produto.DisponivelDelivery = produtoDto.DisponivelDelivery;
        produto.AtualizadoEm = DateTime.UtcNow;

        var produtoAtualizado = await _produtoRepository.UpdateAsync(produto);
        return MapToDto(produtoAtualizado);
    }

    public async Task<ProdutoDto> GetByIdAsync(int id)
    {
        var produto = await _produtoRepository.GetByIdAsync(id);
        if (produto == null)
            throw new ArgumentException("Produto não encontrado");

        return MapToDto(produto);
    }

    public async Task<ProdutoDto?> GetByCodigoBarrasAsync(string codigoBarras)
    {
        var produto = await _produtoRepository.GetByCodigoBarrasAsync(codigoBarras);
        return produto != null ? MapToDto(produto) : null;
    }

    public async Task<IEnumerable<ProdutoListDto>> GetAllAsync()
    {
        var produtos = await _produtoRepository.GetAllAsync();
        return produtos.Select(MapToListDto);
    }

    public async Task<IEnumerable<ProdutoListDto>> GetActiveAsync()
    {
        var produtos = await _produtoRepository.GetActiveAsync();
        return produtos.Select(MapToListDto);
    }

    public async Task<IEnumerable<ProdutoListDto>> GetByTipoAsync(TipoProduto tipo)
    {
        var produtos = await _produtoRepository.GetByTipoAsync(tipo);
        return produtos.Select(MapToListDto);
    }

    public async Task<IEnumerable<ProdutoListDto>> GetByNomeAsync(string nome)
    {
        var produtos = await _produtoRepository.GetByNomeAsync(nome);
        return produtos.Select(MapToListDto);
    }

    public async Task<IEnumerable<ProdutoListDto>> GetComEstoqueBaixoAsync()
    {
        var produtos = await _produtoRepository.GetComEstoqueBaixoAsync();
        return produtos.Select(MapToListDto);
    }

    public async Task<IEnumerable<ProdutoListDto>> GetDisponivelDeliveryAsync()
    {
        var produtos = await _produtoRepository.GetDisponivelDeliveryAsync();
        return produtos.Select(MapToListDto);
    }

    public async Task<ProdutoDto> InativarAsync(int id)
    {
        var produto = await _produtoRepository.GetByIdAsync(id);
        if (produto == null)
            throw new ArgumentException("Produto não encontrado");

        produto.Ativo = false;
        produto.AtualizadoEm = DateTime.UtcNow;

        var produtoAtualizado = await _produtoRepository.UpdateAsync(produto);
        return MapToDto(produtoAtualizado);
    }

    public async Task<ProdutoDto> AtivarAsync(int id)
    {
        var produto = await _produtoRepository.GetByIdAsync(id);
        if (produto == null)
            throw new ArgumentException("Produto não encontrado");

        produto.Ativo = true;
        produto.AtualizadoEm = DateTime.UtcNow;

        var produtoAtualizado = await _produtoRepository.UpdateAsync(produto);
        return MapToDto(produtoAtualizado);
    }

    public async Task<ProdutoDto> AtualizarEstoqueAsync(ProdutoEstoqueDto estoqueDto)
    {
        var produto = await _produtoRepository.GetByIdAsync(estoqueDto.ProdutoId);
        if (produto == null)
            throw new ArgumentException("Produto não encontrado");

        if (produto.ControlaNaoEstoque)
            throw new InvalidOperationException("Produto não controla estoque");

        switch (estoqueDto.TipoMovimento)
        {
            case TipoMovimentoEstoque.Entrada:
                produto.EstoqueAtual += estoqueDto.Quantidade;
                break;
            case TipoMovimentoEstoque.Saida:
                if (produto.EstoqueAtual < estoqueDto.Quantidade)
                    throw new InvalidOperationException("Estoque insuficiente");
                produto.EstoqueAtual -= estoqueDto.Quantidade;
                break;
            case TipoMovimentoEstoque.Ajuste:
                produto.EstoqueAtual = estoqueDto.Quantidade;
                break;
        }

        produto.AtualizadoEm = DateTime.UtcNow;

        var produtoAtualizado = await _produtoRepository.UpdateAsync(produto);
        return MapToDto(produtoAtualizado);
    }

    public async Task DeleteAsync(int id)
    {
        var produto = await _produtoRepository.GetByIdAsync(id);
        if (produto == null)
            throw new ArgumentException("Produto não encontrado");

        await _produtoRepository.DeleteAsync(id);
    }

    public async Task<bool> CodigoBarrasExistsAsync(string codigoBarras, int? excludeId = null)
    {
        var produto = await _produtoRepository.GetByCodigoBarrasAsync(codigoBarras);
        return produto != null && (excludeId == null || produto.Id != excludeId);
    }

    private ProdutoDto MapToDto(Produto produto)
    {
        return new ProdutoDto
        {
            Id = produto.Id,
            Nome = produto.Nome,
            Descricao = produto.Descricao,
            Tipo = produto.Tipo,
            PrecoVenda = produto.PrecoVenda,
            PrecoCusto = produto.PrecoCusto,
            CodigoBarras = produto.CodigoBarras,
            Unidade = produto.Unidade,
            ControlaNaoEstoque = produto.ControlaNaoEstoque,
            EstoqueMinimo = produto.EstoqueMinimo,
            EstoqueAtual = produto.EstoqueAtual,
            CaminhoFoto = produto.CaminhoFoto,
            Ingredientes = produto.Ingredientes,
            TempoPreparoMinutos = produto.TempoPreparoMinutos,
            DisponivelDelivery = produto.DisponivelDelivery,
            CriadoEm = produto.CriadoEm,
            AtualizadoEm = produto.AtualizadoEm,
            Ativo = produto.Ativo
        };
    }

    private ProdutoListDto MapToListDto(Produto produto)
    {
        return new ProdutoListDto
        {
            Id = produto.Id,
            Nome = produto.Nome,
            Descricao = produto.Descricao,
            Tipo = produto.Tipo,
            PrecoVenda = produto.PrecoVenda,
            PrecoCusto = produto.PrecoCusto,
            CodigoBarras = produto.CodigoBarras,
            Unidade = produto.Unidade,
            ControlaNaoEstoque = produto.ControlaNaoEstoque,
            EstoqueAtual = produto.EstoqueAtual,
            EstoqueMinimo = produto.EstoqueMinimo,
            DisponivelDelivery = produto.DisponivelDelivery,
            Ativo = produto.Ativo
        };
    }
}