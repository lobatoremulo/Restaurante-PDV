# Exemplo Pr�tico - M�dulo de Vendas e Pagamentos

Este arquivo cont�m exemplos pr�ticos de como usar o novo m�dulo de vendas e pagamentos implementado no sistema RestaurantePDV.

## Cen�rio: Pagamento de uma Comanda

### 1. Preparar Pagamento de uma Comanda

**Endpoint:** `GET /api/vendas/comanda/123/preparar-pagamento`

**Resposta esperada:**
```json
{
  "id": 123,
  "numeroComanda": "CMD20241210001",
  "mesa": "Mesa 05",
  "clienteNome": "Jo�o Silva",
  "garcomNome": "Maria Santos",
  "valorTotal": 85.50,
  "desconto": 0.00,
  "acrescimo": 0.00,
  "valorFinal": 85.50,
  "dataAbertura": "2024-12-10T18:30:00Z",
  "itens": [
    {
      "id": 1,
      "produtoNome": "Hamb�rguer Especial",
      "quantidade": 1,
      "valorUnitario": 25.90,
      "valorTotal": 25.90,
      "observacoes": "Sem cebola",
      "adicionais": "Bacon extra"
    },
    {
      "id": 2,
      "produtoNome": "Batata Frita Grande",
      "quantidade": 2,
      "valorUnitario": 15.90,
      "valorTotal": 31.80,
      "observacoes": null,
      "adicionais": null
    },
    {
      "id": 3,
      "produtoNome": "Refrigerante 350ml",
      "quantidade": 2,
      "valorUnitario": 6.90,
      "valorTotal": 13.80,
      "observacoes": null,
      "adicionais": null
    },
    {
      "id": 4,
      "produtoNome": "Sobremesa Chocolate",
      "quantidade": 1,
      "valorUnitario": 14.00,
      "valorTotal": 14.00,
      "observacoes": null,
      "adicionais": null
    }
  ],
  "podeSerPaga": true
}
```

### 2. Validar Pagamento (Opcional)

**Endpoint:** `POST /api/vendas/validar-pagamento`

**Request Body:**
```json
{
  "comandaId": 123,
  "desconto": 5.50,
  "acrescimo": 0.00,
  "formasPagamento": [
    {
      "formaPagamento": 1,
      "valor": 50.00,
      "valorRecebido": 50.00,
      "observacoes": "Dinheiro"
    },
    {
      "formaPagamento": 2,
      "valor": 30.00,
      "numeroDocumento": "1234****5678",
      "observacoes": "Cart�o de Cr�dito"
    }
  ],
  "operadorId": 1
}
```

**Resposta esperada:**
```json
{
  "valido": true,
  "troco": 0.00,
  "valorFinal": 80.00
}
```

### 3. Processar Pagamento da Comanda

**Endpoint:** `POST /api/vendas/pagar-comanda`

**Request Body:**
```json
{
  "comandaId": 123,
  "desconto": 5.50,
  "acrescimo": 0.00,
  "observacoes": "Pagamento processado com sucesso",
  "formasPagamento": [
    {
      "formaPagamento": 1,
      "valor": 50.00,
      "valorRecebido": 50.00,
      "observacoes": "Dinheiro"
    },
    {
      "formaPagamento": 2,
      "valor": 30.00,
      "numeroDocumento": "1234****5678",
      "observacoes": "Cart�o de Cr�dito Visa"
    }
  ],
  "operadorId": 1
}
```

**Resposta esperada:**
```json
{
  "id": 456,
  "numeroVenda": "VND20241210001",
  "comandaId": 123,
  "numeroComanda": "CMD20241210001",
  "dataVenda": "2024-12-10T20:15:00Z",
  "status": 2,
  "subTotal": 85.50,
  "desconto": 5.50,
  "acrescimo": 0.00,
  "valorTotal": 80.00,
  "valorPago": 80.00,
  "troco": 0.00,
  "observacoes": "Pagamento processado com sucesso",
  "operadorNome": "Carlos Operador",
  "temTroco": false,
  "pagamentoCompleto": true,
  "pagamentos": [
    {
      "id": 1,
      "formaPagamento": 1,
      "formaPagamentoDescricao": "Dinheiro",
      "valor": 50.00,
      "valorRecebido": 50.00,
      "troco": 0.00,
      "numeroDocumento": null,
      "observacoes": "Dinheiro",
      "dataPagamento": "2024-12-10T20:15:00Z"
    },
    {
      "id": 2,
      "formaPagamento": 2,
      "formaPagamentoDescricao": "CartaoCredito",
      "valor": 30.00,
      "valorRecebido": null,
      "troco": 0.00,
      "numeroDocumento": "1234****5678",
      "observacoes": "Cart�o de Cr�dito Visa",
      "dataPagamento": "2024-12-10T20:15:00Z"
    }
  ],
  "itens": [
    {
      "id": 1,
      "vendaId": 456,
      "produtoId": 10,
      "produtoNome": "Hamb�rguer Especial",
      "quantidade": 1,
      "valorUnitario": 25.90,
      "desconto": 0.00,
      "valorTotal": 25.90,
      "observacoes": "Sem cebola",
      "adicionais": "Bacon extra"
    }
    // ... outros itens
  ]
}
```

## Cen�rios Adicionais

### 4. Listar Comandas Pendentes de Pagamento

**Endpoint:** `GET /api/vendas/comandas-pendentes`

**Resposta esperada:**
```json
[
  {
    "id": 124,
    "numeroComanda": "CMD20241210002",
    "mesa": "Mesa 08",
    "clienteNome": "Ana Costa",
    "garcomNome": "Pedro Lima",
    "valorTotal": 65.90,
    "desconto": 0.00,
    "acrescimo": 0.00,
    "valorFinal": 65.90,
    "dataAbertura": "2024-12-10T19:00:00Z",
    "podeSerPaga": true
  },
  {
    "id": 125,
    "numeroComanda": "CMD20241210003",
    "mesa": "Mesa 12",
    "clienteNome": null,
    "garcomNome": "Maria Santos",
    "valorTotal": 45.50,
    "desconto": 0.00,
    "acrescimo": 2.50,
    "valorFinal": 48.00,
    "dataAbertura": "2024-12-10T19:15:00Z",
    "podeSerPaga": true
  }
]
```

### 5. Consultar Venda Espec�fica

**Endpoint:** `GET /api/vendas/venda-comanda/456`

**Resposta:** A mesma estrutura do item 3, com todos os detalhes da venda.

### 6. Relat�rio de Vendas

**Endpoint:** `GET /api/vendas/relatorio-vendas?dataInicio=2024-12-10&dataFim=2024-12-10`

**Resposta esperada:**
```json
{
  "periodo": {
    "dataInicio": "2024-12-10T00:00:00Z",
    "dataFim": "2024-12-10T23:59:59Z"
  },
  "totalVendas": 15,
  "valorTotal": 1250.80,
  "valorPago": 1250.80,
  "trocoTotal": 25.20,
  "vendasPorFormaPagamento": [
    {
      "formaPagamento": "Dinheiro",
      "quantidade": 8,
      "valor": 650.40
    },
    {
      "formaPagamento": "CartaoCredito",
      "quantidade": 5,
      "valor": 400.30
    },
    {
      "formaPagamento": "CartaoDebito",
      "quantidade": 3,
      "valor": 150.10
    },
    {
      "formaPagamento": "Pix",
      "quantidade": 2,
      "valor": 50.00
    }
  ],
  "vendasPorStatus": [
    {
      "status": "Finalizada",
      "quantidade": 15,
      "valor": 1250.80
    }
  ]
}
```

## Fluxo Completo Integrado

### 1. Cliente faz pedido ? Comanda � criada
### 2. Itens s�o adicionados � comanda
### 3. Comanda � fechada quando pedido est� completo
### 4. Sistema prepara dados para pagamento
### 5. Operador processa pagamento (m�ltiplas formas se necess�rio)
### 6. Sistema:
   - Cria venda automaticamente
   - Registra movimentos no caixa
   - Processa baixa no estoque
   - Gera comprovante

## Benef�cios da Implementa��o

1. **Flexibilidade Total**: Suporte a m�ltiplas formas de pagamento em uma �nica transa��o
2. **Integra��o Autom�tica**: Conecta vendas, caixa e estoque automaticamente
3. **Auditoria Completa**: Rastreabilidade total de todas as opera��es
4. **APIs Bem Estruturadas**: Endpoints claros e bem documentados
5. **Tratamento de Erros**: Valida��es robustas em todos os pontos
6. **Escalabilidade**: Arquitetura preparada para expans�es futuras

## Estados das Entidades

- **Comanda**: Aberta ? Fechada ? (Paga via Venda)
- **Venda**: Criada j� Finalizada (para comandas)
- **Caixa**: Recebe movimentos automaticamente
- **Estoque**: Baixa autom�tica ap�s finaliza��o da venda

Este m�dulo transforma o sistema em uma solu��o completa de PDV, mantendo a integridade dos dados e proporcionando uma experi�ncia rica tanto para operadores quanto para gestores.