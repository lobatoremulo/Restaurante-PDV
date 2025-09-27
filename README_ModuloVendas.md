# M�dulo de Vendas e Pagamentos

Este documento descreve o novo m�dulo de Vendas e Pagamentos implementado no sistema RestaurantePDV, que permite o processamento avan�ado de pagamentos de comandas com m�ltiplas formas de pagamento.

## Funcionalidades Implementadas

### 1. Pagamento de Comandas
- **Endpoint**: `POST /api/vendas/pagar-comanda`
- **Funcionalidade**: Processa o pagamento de uma comanda fechada com m�ltiplas formas de pagamento
- **Recursos**:
  - Suporte a m�ltiplas formas de pagamento em uma �nica transa��o
  - C�lculo autom�tico de troco
  - Valida��o de valores
  - Integra��o autom�tica com o controle de caixa
  - Baixa autom�tica no estoque

### 2. Prepara��o de Pagamento
- **Endpoint**: `GET /api/vendas/comanda/{comandaId}/preparar-pagamento`
- **Funcionalidade**: Prepara os dados de uma comanda para pagamento
- **Retorna**: Informa��es detalhadas da comanda, itens e valores

### 3. Valida��o de Pagamento
- **Endpoint**: `POST /api/vendas/validar-pagamento`
- **Funcionalidade**: Valida um pagamento antes de process�-lo
- **Recursos**:
  - Verifica��o de valores
  - C�lculo de troco
  - Valida��o de regras de neg�cio

### 4. Comandas Pendentes
- **Endpoint**: `GET /api/vendas/comandas-pendentes`
- **Funcionalidade**: Lista todas as comandas fechadas que ainda n�o foram pagas

### 5. Relat�rios de Vendas
- **Endpoint**: `GET /api/vendas/relatorio-vendas`
- **Funcionalidade**: Gera relat�rios detalhados de vendas por per�odo
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
Nova entidade para gerenciar m�ltiplas formas de pagamento em uma venda:
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

### 1. Prepara��o
```http
GET /api/vendas/comanda/123/preparar-pagamento
```

### 2. Valida��o (Opcional)
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

## Integra��o com Caixa

O m�dulo se integra automaticamente com o sistema de controle de caixa:

1. **Valida��o**: Verifica se h� um caixa aberto antes de processar o pagamento
2. **Movimenta��o**: Cria movimentos de caixa para cada forma de pagamento
3. **Categoriza��o**: Classifica os movimentos como "Venda" no sistema de caixa
4. **Rastreabilidade**: Vincula cada movimento � venda correspondente

## Recursos Avan�ados

### M�ltiplas Formas de Pagamento
- Uma venda pode ser paga com diferentes formas de pagamento
- Cada forma de pagamento � registrada individualmente
- C�lculo autom�tico de troco para pagamentos em dinheiro

### Divis�o de Conta
- Suporte futuro para dividir uma conta entre m�ltiplos clientes
- Estrutura preparada para implementa��o de divis�o proporcional

### Reprocessamento de Pagamentos
- Funcionalidade para reprocessar pagamentos com erro
- Endpoint: `POST /api/vendas/reprocessar-pagamento/{vendaId}`

### Controle de Estoque
- Baixa autom�tica no estoque ap�s finaliza��o da venda
- Cria��o de movimentos de estoque para rastreabilidade

## Valida��es Implementadas

1. **Caixa Aberto**: Verifica se h� um caixa aberto para processar vendas
2. **Status da Comanda**: Apenas comandas fechadas podem ser pagas
3. **Valores**: Valida��o de que o valor pago � suficiente
4. **Produtos**: Verifica��o da exist�ncia dos produtos
5. **Pagamento �nico**: Impede pagamento duplicado da mesma comanda

## Benef�cios do M�dulo

1. **Flexibilidade**: Suporte a m�ltiplas formas de pagamento
2. **Automa��o**: Integra��o autom�tica com caixa e estoque
3. **Auditoria**: Rastreabilidade completa das transa��es
4. **Usabilidade**: APIs bem estruturadas e documentadas
5. **Escalabilidade**: Arquitetura preparada para expans�es futuras

Este m�dulo transforma o fluxo de comandas em um sistema completo de vendas, mantendo a integridade dos dados e proporcionando uma experi�ncia rica tanto para o usu�rio quanto para o sistema de gest�o.