# Módulo de Vendas e Pagamentos

Este documento descreve o novo módulo de Vendas e Pagamentos implementado no sistema RestaurantePDV, que permite o processamento avançado de pagamentos de comandas com múltiplas formas de pagamento.

## Funcionalidades Implementadas

### 1. Pagamento de Comandas
- **Endpoint**: `POST /api/vendas/pagar-comanda`
- **Funcionalidade**: Processa o pagamento de uma comanda fechada com múltiplas formas de pagamento
- **Recursos**:
  - Suporte a múltiplas formas de pagamento em uma única transação
  - Cálculo automático de troco
  - Validação de valores
  - Integração automática com o controle de caixa
  - Baixa automática no estoque

### 2. Preparação de Pagamento
- **Endpoint**: `GET /api/vendas/comanda/{comandaId}/preparar-pagamento`
- **Funcionalidade**: Prepara os dados de uma comanda para pagamento
- **Retorna**: Informações detalhadas da comanda, itens e valores

### 3. Validação de Pagamento
- **Endpoint**: `POST /api/vendas/validar-pagamento`
- **Funcionalidade**: Valida um pagamento antes de processá-lo
- **Recursos**:
  - Verificação de valores
  - Cálculo de troco
  - Validação de regras de negócio

### 4. Comandas Pendentes
- **Endpoint**: `GET /api/vendas/comandas-pendentes`
- **Funcionalidade**: Lista todas as comandas fechadas que ainda não foram pagas

### 5. Relatórios de Vendas
- **Endpoint**: `GET /api/vendas/relatorio-vendas`
- **Funcionalidade**: Gera relatórios detalhados de vendas por período
- **Recursos**:
  - Agrupamento por forma de pagamento
  - Agrupamento por status
  - Totalizadores gerais

## DTOs Criados

### PagamentoComandaDto
```csharp
public class PagamentoComandaDto
{
    public int ComandaId { get; set; }
    public decimal Desconto { get; set; }
    public decimal Acrescimo { get; set; }
    public string? Observacoes { get; set; }
    public List<FormaPagamentoDto> FormasPagamento { get; set; }
    public int OperadorId { get; set; }
    public List<DivisaoContaDto>? DivisoesConta { get; set; }
}
```

### FormaPagamentoDto
```csharp
public class FormaPagamentoDto
{
    public FormaPagamento FormaPagamento { get; set; }
    public decimal Valor { get; set; }
    public decimal? ValorRecebido { get; set; }
    public string? NumeroDocumento { get; set; }
    public string? Observacoes { get; set; }
}
```

### VendaComandaDto
```csharp
public class VendaComandaDto
{
    public int Id { get; set; }
    public string NumeroVenda { get; set; }
    public int ComandaId { get; set; }
    public string NumeroComanda { get; set; }
    public DateTime DataVenda { get; set; }
    public StatusVenda Status { get; set; }
    public decimal SubTotal { get; set; }
    public decimal Desconto { get; set; }
    public decimal Acrescimo { get; set; }
    public decimal ValorTotal { get; set; }
    public decimal ValorPago { get; set; }
    public decimal Troco { get; set; }
    public string? Observacoes { get; set; }
    public string OperadorNome { get; set; }
    public List<PagamentoVendaDto> Pagamentos { get; set; }
    public List<ItemVendaDto> Itens { get; set; }
}
```

## Entidades Criadas

### PagamentoVenda
Nova entidade para gerenciar múltiplas formas de pagamento em uma venda:
```csharp
public class PagamentoVenda : BaseEntity
{
    public int VendaId { get; set; }
    public FormaPagamento FormaPagamento { get; set; }
    public decimal Valor { get; set; }
    public decimal? ValorRecebido { get; set; }
    public string? NumeroDocumento { get; set; }
    public string? Observacoes { get; set; }
    public DateTime DataPagamento { get; set; }
    public decimal Troco { get; }
    public virtual Venda Venda { get; set; }
}
```

## Fluxo de Pagamento

### 1. Preparação
```http
GET /api/vendas/comanda/123/preparar-pagamento
```

### 2. Validação (Opcional)
```http
POST /api/vendas/validar-pagamento
Content-Type: application/json

{
  "comandaId": 123,
  "desconto": 0,
  "acrescimo": 0,
  "formasPagamento": [
    {
      "formaPagamento": 1,
      "valor": 50.00,
      "valorRecebido": 60.00
    }
  ],
  "operadorId": 1
}
```

### 3. Processamento
```http
POST /api/vendas/pagar-comanda
Content-Type: application/json

{
  "comandaId": 123,
  "desconto": 0,
  "acrescimo": 0,
  "observacoes": "Pagamento processado",
  "formasPagamento": [
    {
      "formaPagamento": 1,
      "valor": 50.00,
      "valorRecebido": 60.00,
      "observacoes": "Dinheiro"
    }
  ],
  "operadorId": 1
}
```

## Integração com Caixa

O módulo se integra automaticamente com o sistema de controle de caixa:

1. **Validação**: Verifica se há um caixa aberto antes de processar o pagamento
2. **Movimentação**: Cria movimentos de caixa para cada forma de pagamento
3. **Categorização**: Classifica os movimentos como "Venda" no sistema de caixa
4. **Rastreabilidade**: Vincula cada movimento à venda correspondente

## Recursos Avançados

### Múltiplas Formas de Pagamento
- Uma venda pode ser paga com diferentes formas de pagamento
- Cada forma de pagamento é registrada individualmente
- Cálculo automático de troco para pagamentos em dinheiro

### Divisão de Conta
- Suporte futuro para dividir uma conta entre múltiplos clientes
- Estrutura preparada para implementação de divisão proporcional

### Reprocessamento de Pagamentos
- Funcionalidade para reprocessar pagamentos com erro
- Endpoint: `POST /api/vendas/reprocessar-pagamento/{vendaId}`

### Controle de Estoque
- Baixa automática no estoque após finalização da venda
- Criação de movimentos de estoque para rastreabilidade

## Validações Implementadas

1. **Caixa Aberto**: Verifica se há um caixa aberto para processar vendas
2. **Status da Comanda**: Apenas comandas fechadas podem ser pagas
3. **Valores**: Validação de que o valor pago é suficiente
4. **Produtos**: Verificação da existência dos produtos
5. **Pagamento Único**: Impede pagamento duplicado da mesma comanda

## Benefícios do Módulo

1. **Flexibilidade**: Suporte a múltiplas formas de pagamento
2. **Automação**: Integração automática com caixa e estoque
3. **Auditoria**: Rastreabilidade completa das transações
4. **Usabilidade**: APIs bem estruturadas e documentadas
5. **Escalabilidade**: Arquitetura preparada para expansões futuras

Este módulo transforma o fluxo de comandas em um sistema completo de vendas, mantendo a integridade dos dados e proporcionando uma experiência rica tanto para o usuário quanto para o sistema de gestão.