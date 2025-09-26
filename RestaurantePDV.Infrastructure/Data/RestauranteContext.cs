using Microsoft.EntityFrameworkCore;
using RestaurantePDV.Domain.Entities;

namespace RestaurantePDV.Infrastructure.Data;

public class RestauranteContext : DbContext
{
    public RestauranteContext(DbContextOptions<RestauranteContext> options) : base(options)
    {
    }

    public DbSet<Cliente> Clientes { get; set; }
    public DbSet<Produto> Produtos { get; set; }
    public DbSet<Insumo> Insumos { get; set; }
    public DbSet<Fornecedor> Fornecedores { get; set; }
    public DbSet<FichaTecnica> FichasTecnicas { get; set; }
    public DbSet<MovimentoEstoque> MovimentosEstoque { get; set; }
    public DbSet<Venda> Vendas { get; set; }
    public DbSet<ItemVenda> ItensVenda { get; set; }
    public DbSet<Comanda> Comandas { get; set; }
    public DbSet<ItemComanda> ItensComanda { get; set; }
    public DbSet<ContaPagar> ContasPagar { get; set; }
    public DbSet<ContaReceber> ContasReceber { get; set; }
    
    // Novas entidades do sistema administrativo
    public DbSet<Funcionario> Funcionarios { get; set; }
    public DbSet<EscalaTrabalho> EscalasTrabalho { get; set; }
    public DbSet<ClienteRestricao> ClientesRestricoes { get; set; }

    // Entidades do Controle de Caixa
    public DbSet<Caixa> Caixas { get; set; }
    public DbSet<MovimentoCaixa> MovimentosCaixa { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configurações para Cliente
        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nome).IsRequired().HasMaxLength(100);
            entity.Property(e => e.CpfCnpj).HasMaxLength(14);
            entity.Property(e => e.Telefone).HasMaxLength(20);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Endereco).HasMaxLength(200);
            entity.Property(e => e.Cidade).HasMaxLength(50);
            entity.Property(e => e.Estado).HasMaxLength(2);
            entity.Property(e => e.Cep).HasMaxLength(9);
            entity.Property(e => e.Observacoes).HasMaxLength(500);
            entity.Property(e => e.LimiteCredito).HasPrecision(18, 2);

            entity.HasIndex(e => e.CpfCnpj).IsUnique().HasFilter($"\"{nameof(Cliente.CpfCnpj)}\" IS NOT NULL");
            entity.HasIndex(e => e.Email).IsUnique().HasFilter($"\"{nameof(Cliente.Email)}\" IS NOT NULL");
        });

        // Configurações para Funcionario
        modelBuilder.Entity<Funcionario>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nome).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Cpf).IsRequired().HasMaxLength(14);
            entity.Property(e => e.Rg).HasMaxLength(15);
            entity.Property(e => e.Telefone).HasMaxLength(20);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Cargo).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Setor).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Endereco).HasMaxLength(200);
            entity.Property(e => e.Cidade).HasMaxLength(50);
            entity.Property(e => e.Estado).HasMaxLength(2);
            entity.Property(e => e.Cep).HasMaxLength(9);
            entity.Property(e => e.Observacoes).HasMaxLength(500);
            entity.Property(e => e.Salario).HasPrecision(18, 2);

            entity.HasIndex(e => e.Cpf).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique().HasFilter($"\"{nameof(Funcionario.Email)}\" IS NOT NULL");
        });

        // Configurações para EscalaTrabalho
        modelBuilder.Entity<EscalaTrabalho>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Observacoes).HasMaxLength(200);

            entity.HasOne(e => e.Funcionario)
                .WithMany(f => f.Escalas)
                .HasForeignKey(e => e.FuncionarioId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => new { e.FuncionarioId, e.DataEscala }).IsUnique();
        });

        // Configurações para ClienteRestricao
        modelBuilder.Entity<ClienteRestricao>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Descricao).IsRequired().HasMaxLength(500);
            entity.Property(e => e.ObservacoesRemocao).HasMaxLength(500);

            entity.HasOne(e => e.Cliente)
                .WithMany(c => c.Restricoes)
                .HasForeignKey(e => e.ClienteId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.ResponsavelInclusao)
                .WithMany()
                .HasForeignKey(e => e.ResponsavelInclusaoId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.ResponsavelRemocao)
                .WithMany()
                .HasForeignKey(e => e.ResponsavelRemocaoId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configurações para Produto
        modelBuilder.Entity<Produto>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nome).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Descricao).HasMaxLength(500);
            entity.Property(e => e.PrecoVenda).HasPrecision(18, 2);
            entity.Property(e => e.PrecoCusto).HasPrecision(18, 2);
            entity.Property(e => e.CodigoBarras).HasMaxLength(50);
            entity.Property(e => e.Unidade).HasMaxLength(20);
            entity.Property(e => e.EstoqueMinimo).HasPrecision(18, 3);
            entity.Property(e => e.EstoqueAtual).HasPrecision(18, 3);
            entity.Property(e => e.CaminhoFoto).HasMaxLength(200);
            entity.Property(e => e.Ingredientes).HasMaxLength(500);

            entity.HasIndex(e => e.CodigoBarras).IsUnique().HasFilter($"\"{nameof(Produto.CodigoBarras)}\" IS NOT NULL");
        });

        // Configurações para Fornecedor
        modelBuilder.Entity<Fornecedor>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.RazaoSocial).IsRequired().HasMaxLength(100);
            entity.Property(e => e.NomeFantasia).HasMaxLength(100);
            entity.Property(e => e.CnpjCpf).IsRequired().HasMaxLength(18);
            entity.Property(e => e.InscricaoEstadual).HasMaxLength(20);
            entity.Property(e => e.Telefone).HasMaxLength(20);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Endereco).HasMaxLength(200);
            entity.Property(e => e.Cidade).HasMaxLength(50);
            entity.Property(e => e.Estado).HasMaxLength(2);
            entity.Property(e => e.Cep).HasMaxLength(9);
            entity.Property(e => e.Contato).HasMaxLength(100);
            entity.Property(e => e.Observacoes).HasMaxLength(500);

            entity.HasIndex(e => e.CnpjCpf).IsUnique();
        });

        // Configurações para Insumo
        modelBuilder.Entity<Insumo>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nome).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Descricao).HasMaxLength(500);
            entity.Property(e => e.EstoqueAtual).HasPrecision(18, 3);
            entity.Property(e => e.EstoqueMinimo).HasPrecision(18, 3);
            entity.Property(e => e.PrecoCusto).HasPrecision(18, 2);
            entity.Property(e => e.CodigoInterno).HasMaxLength(50);
            entity.Property(e => e.Lote).HasMaxLength(100);

            entity.HasOne(e => e.Fornecedor)
                .WithMany(f => f.Insumos)
                .HasForeignKey(e => e.FornecedorId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Configurações para FichaTecnica
        modelBuilder.Entity<FichaTecnica>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Quantidade).HasPrecision(18, 3);
            entity.Property(e => e.Observacoes).HasMaxLength(200);

            entity.HasOne(e => e.Produto)
                .WithMany(p => p.FichasTecnicas)
                .HasForeignKey(e => e.ProdutoId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Insumo)
                .WithMany(i => i.FichasTecnicas)
                .HasForeignKey(e => e.InsumoId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => new { e.ProdutoId, e.InsumoId }).IsUnique();
        });

        // Configurações para MovimentoEstoque
        modelBuilder.Entity<MovimentoEstoque>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Quantidade).HasPrecision(18, 3);
            entity.Property(e => e.ValorUnitario).HasPrecision(18, 2);
            entity.Property(e => e.Observacoes).HasMaxLength(200);
            entity.Property(e => e.NumeroDocumento).HasMaxLength(100);

            entity.HasOne(e => e.Produto)
                .WithMany()
                .HasForeignKey(e => e.ProdutoId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.Insumo)
                .WithMany(i => i.MovimentosEstoque)
                .HasForeignKey(e => e.InsumoId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.Venda)
                .WithMany(v => v.MovimentosEstoque)
                .HasForeignKey(e => e.VendaId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Configurações para Venda
        modelBuilder.Entity<Venda>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.NumeroVenda).IsRequired().HasMaxLength(20);
            entity.Property(e => e.SubTotal).HasPrecision(18, 2);
            entity.Property(e => e.Desconto).HasPrecision(18, 2);
            entity.Property(e => e.Acrescimo).HasPrecision(18, 2);
            entity.Property(e => e.ValorTotal).HasPrecision(18, 2);
            entity.Property(e => e.ValorPago).HasPrecision(18, 2);
            entity.Property(e => e.Troco).HasPrecision(18, 2);
            entity.Property(e => e.Observacoes).HasMaxLength(500);

            entity.HasOne(e => e.Cliente)
                .WithMany(c => c.Vendas)
                .HasForeignKey(e => e.ClienteId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.Comanda)
                .WithMany(c => c.Vendas)
                .HasForeignKey(e => e.ComandaId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasIndex(e => e.NumeroVenda).IsUnique();
        });

        // Configurações para ItemVenda
        modelBuilder.Entity<ItemVenda>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Quantidade).HasPrecision(18, 3);
            entity.Property(e => e.ValorUnitario).HasPrecision(18, 2);
            entity.Property(e => e.Desconto).HasPrecision(18, 2);
            entity.Property(e => e.ValorTotal).HasPrecision(18, 2);
            entity.Property(e => e.Observacoes).HasMaxLength(500);
            entity.Property(e => e.Adicionais).HasMaxLength(500);

            entity.HasOne(e => e.Venda)
                .WithMany(v => v.Itens)
                .HasForeignKey(e => e.VendaId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Produto)
                .WithMany(p => p.ItensVenda)
                .HasForeignKey(e => e.ProdutoId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configurações para Comanda
        modelBuilder.Entity<Comanda>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.NumeroComanda).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Mesa).HasMaxLength(50);
            entity.Property(e => e.ValorTotal).HasPrecision(18, 2);
            entity.Property(e => e.Desconto).HasPrecision(18, 2);
            entity.Property(e => e.Acrescimo).HasPrecision(18, 2);
            entity.Property(e => e.ValorFinal).HasPrecision(18, 2);
            entity.Property(e => e.Observacoes).HasMaxLength(500);

            entity.HasOne(e => e.Cliente)
                .WithMany(c => c.Comandas)
                .HasForeignKey(e => e.ClienteId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.Garcom)
                .WithMany(f => f.Comandas)
                .HasForeignKey(e => e.GarcomId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasIndex(e => e.NumeroComanda).IsUnique();
        });

        // Configurações para ItemComanda
        modelBuilder.Entity<ItemComanda>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Quantidade).HasPrecision(18, 3);
            entity.Property(e => e.ValorUnitario).HasPrecision(18, 2);
            entity.Property(e => e.ValorTotal).HasPrecision(18, 2);
            entity.Property(e => e.Observacoes).HasMaxLength(500);
            entity.Property(e => e.Adicionais).HasMaxLength(500);

            entity.HasOne(e => e.Comanda)
                .WithMany(c => c.Itens)
                .HasForeignKey(e => e.ComandaId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Produto)
                .WithMany(p => p.ItensComanda)
                .HasForeignKey(e => e.ProdutoId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configurações para ContaPagar
        modelBuilder.Entity<ContaPagar>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Descricao).IsRequired().HasMaxLength(100);
            entity.Property(e => e.ValorOriginal).HasPrecision(18, 2);
            entity.Property(e => e.ValorPago).HasPrecision(18, 2);
            entity.Property(e => e.ValorDesconto).HasPrecision(18, 2);
            entity.Property(e => e.ValorJuros).HasPrecision(18, 2);
            entity.Property(e => e.NumeroDocumento).HasMaxLength(100);
            entity.Property(e => e.Observacoes).HasMaxLength(500);

            entity.HasOne(e => e.Fornecedor)
                .WithMany(f => f.ContasPagar)
                .HasForeignKey(e => e.FornecedorId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Configurações para ContaReceber
        modelBuilder.Entity<ContaReceber>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Descricao).IsRequired().HasMaxLength(100);
            entity.Property(e => e.ValorOriginal).HasPrecision(18, 2);
            entity.Property(e => e.ValorRecebido).HasPrecision(18, 2);
            entity.Property(e => e.ValorDesconto).HasPrecision(18, 2);
            entity.Property(e => e.ValorJuros).HasPrecision(18, 2);
            entity.Property(e => e.NumeroDocumento).HasMaxLength(100);
            entity.Property(e => e.Observacoes).HasMaxLength(500);

            entity.HasOne(e => e.Cliente)
                .WithMany()
                .HasForeignKey(e => e.ClienteId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.Venda)
                .WithMany()
                .HasForeignKey(e => e.VendaId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Configurações para Caixa
        modelBuilder.Entity<Caixa>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ValorAbertura).HasPrecision(18, 2);
            entity.Property(e => e.ValorFechamento).HasPrecision(18, 2);
            entity.Property(e => e.TotalVendas).HasPrecision(18, 2);
            entity.Property(e => e.TotalSangrias).HasPrecision(18, 2);
            entity.Property(e => e.TotalSuprimentos).HasPrecision(18, 2);
            entity.Property(e => e.ObservacoesAbertura).HasMaxLength(500);
            entity.Property(e => e.ObservacoesFechamento).HasMaxLength(500);

            entity.HasOne(e => e.OperadorAbertura)
                .WithMany()
                .HasForeignKey(e => e.OperadorAberturaId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.OperadorFechamento)
                .WithMany()
                .HasForeignKey(e => e.OperadorFechamentoId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configurações para MovimentoCaixa
        modelBuilder.Entity<MovimentoCaixa>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Valor).HasPrecision(18, 2);
            entity.Property(e => e.Descricao).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Observacoes).HasMaxLength(500);
            entity.Property(e => e.NumeroDocumento).HasMaxLength(100);

            entity.HasOne(e => e.Caixa)
                .WithMany(c => c.Movimentos)
                .HasForeignKey(e => e.CaixaId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Venda)
                .WithMany()
                .HasForeignKey(e => e.VendaId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.Operador)
                .WithMany()
                .HasForeignKey(e => e.OperadorId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker
            .Entries()
            .Where(e => e.Entity is Domain.Common.BaseEntity && 
                       (e.State == EntityState.Added || e.State == EntityState.Modified));

        foreach (var entityEntry in entries)
        {
            var entity = (Domain.Common.BaseEntity)entityEntry.Entity;
            
            if (entityEntry.State == EntityState.Added)
            {
                entity.CriadoEm = DateTime.UtcNow;
            }
            else if (entityEntry.State == EntityState.Modified)
            {
                entity.AtualizadoEm = DateTime.UtcNow;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}