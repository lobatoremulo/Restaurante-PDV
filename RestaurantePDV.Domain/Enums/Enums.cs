namespace RestaurantePDV.Domain.Enums;

public enum StatusComanda
{
    Aberta = 1,
    Fechada = 2,
    Cancelada = 3
}

public enum TipoProduto
{
    Bebida = 1,
    Prato = 2,
    Sobremesa = 3,
    Entrada = 4
}

public enum UnidadeMedida
{
    Unitario = 1,
    Quilograma = 2,
    Litro = 3,
    Grama = 4,
    Mililitro = 5
}

public enum TipoMovimentoEstoque
{
    Entrada = 1,
    Saida = 2,
    Ajuste = 3
}

public enum StatusVenda
{
    Aberta = 1,
    Finalizada = 2,
    Cancelada = 3
}

public enum FormaPagamento
{
    Dinheiro = 1,
    CartaoCredito = 2,
    CartaoDebito = 3,
    Pix = 4,
    Fiado = 5
}

public enum StatusConta
{
    Pendente = 1,
    Paga = 2,
    Vencida = 3,
    Cancelada = 4
}

// Novos enums para o sistema administrativo
public enum NivelAcesso
{
    UsuarioComum = 1,
    Gerente = 2,
    Admin = 3
}

public enum TurnoTrabalho
{
    Manha = 1,
    Tarde = 2,
    Noite = 3,
    Integral = 4
}

public enum StatusFuncionario
{
    Ativo = 1,
    Inativo = 2,
    Ferias = 3,
    Afastado = 4
}

public enum MotivoRestricaoCliente
{
    Inadimplencia = 1,
    ComportamentoInadequado = 2,
    FraudeIdentificada = 3,
    SolicitacaoCliente = 4,
    Outros = 5
}

// Enums para Controle de Caixa
public enum StatusCaixa
{
    Aberto = 1,
    Fechado = 2,
    Bloqueado = 3
}

public enum TipoMovimentoCaixa
{
    entrada = 1,
    Saida = 2,
    Venda = 3,
    Sangria = 4,
    Suprimento = 5,
    Abertura = 6,
    Fechamento = 7
}