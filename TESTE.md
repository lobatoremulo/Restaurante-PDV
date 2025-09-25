# Testando a API RestaurantePDV

## Instruções para teste da API

### 1. Configurar Banco de Dados PostgreSQL

```bash
# Criar banco de dados
createdb RestaurantePDV

# Ou via SQL
psql -U postgres -c "CREATE DATABASE RestaurantePDV;"
```

### 2. Executar Migrations

```bash
dotnet ef database update --project RestaurantePDV.Infrastructure --startup-project RestaurantePDV.API
```

### 3. Executar a Aplicação

```bash
dotnet run --project RestaurantePDV.API
```

### 4. Acessar Swagger UI

Abra no navegador: https://localhost:5001/swagger

### 5. Exemplos de Teste via cURL

#### Login
```bash
curl -X POST "https://localhost:5001/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{
    "username": "admin",
    "password": "123456"
  }' -k
```

#### Criar Cliente
```bash
curl -X POST "https://localhost:5001/api/clientes" \
  -H "Authorization: Bearer SEU_TOKEN_AQUI" \
  -H "Content-Type: application/json" \
  -d '{
    "nome": "João da Silva",
    "cpfCnpj": "12345678901",
    "telefone": "(11) 99999-9999",
    "email": "joao@exemplo.com",
    "endereco": "Rua das Flores, 123",
    "cidade": "São Paulo",
    "estado": "SP",
    "cep": "01234567",
    "limiteCredito": 500.00
  }' -k
```

#### Abrir Comanda
```bash
curl -X POST "https://localhost:5001/api/comandas" \
  -H "Authorization: Bearer SEU_TOKEN_AQUI" \
  -H "Content-Type: application/json" \
  -d '{
    "mesa": "Mesa 05",
    "observacoes": "Cliente VIP"
  }' -k
```

#### Criar Venda
```bash
curl -X POST "https://localhost:5001/api/vendas" \
  -H "Authorization: Bearer SEU_TOKEN_AQUI" \
  -H "Content-Type: application/json" \
  -d '{
    "formaPagamento": 1,
    "vendaBalcao": true,
    "itens": [
      {
        "produtoId": 1,
        "quantidade": 2,
        "observacoes": "Sem cebola"
      }
    ]
  }' -k
```

### 6. Scripts SQL para Dados de Teste

Execute no PostgreSQL após as migrations:

```sql
-- Inserir produtos de exemplo
INSERT INTO "Produtos" ("Nome", "Descricao", "Tipo", "PrecoVenda", "Unidade", "EstoqueAtual", "CriadoEm", "Ativo") VALUES
('Hambúrguer Clássico', 'Hambúrguer com carne, alface, tomate e queijo', 2, 25.90, 'UN', 50, NOW(), true),
('Refrigerante Cola', 'Refrigerante sabor cola 350ml', 1, 5.50, 'UN', 100, NOW(), true),
('Batata Frita', 'Porção de batata frita crocante', 4, 12.90, 'UN', 30, NOW(), true),
('Sorvete de Chocolate', 'Sorvete cremoso sabor chocolate', 3, 8.90, 'UN', 20, NOW(), true);

-- Inserir cliente de exemplo
INSERT INTO "Clientes" ("Nome", "Telefone", "Email", "CriadoEm", "Ativo") VALUES
('Cliente Exemplo', '(11) 99999-9999', 'cliente@exemplo.com', NOW(), true);
```

### 7. Testando Funcionalidades Principais

1. **Autenticação**: Use o endpoint `/api/auth/login`
2. **Clientes**: CRUD completo em `/api/clientes`
3. **Comandas**: Fluxo completo de abrir/fechar em `/api/comandas`
4. **Vendas**: Processo de venda em `/api/vendas`

### 8. Verificação no Swagger

- Acesse https://localhost:5001/swagger
- Clique em "Authorize" e cole o token JWT
- Teste todos os endpoints disponíveis

### 9. Status dos Enums

#### TipoProduto
- 1: Bebida
- 2: Prato  
- 3: Sobremesa
- 4: Entrada

#### FormaPagamento
- 1: Dinheiro
- 2: Cartão de Crédito
- 3: Cartão de Débito
- 4: PIX
- 5: Fiado

#### StatusComanda
- 1: Aberta
- 2: Fechada
- 3: Cancelada

#### StatusVenda
- 1: Aberta
- 2: Finalizada
- 3: Cancelada

### 10. Logs e Depuração

A aplicação registra logs no console. Para depuração mais detalhada, configure o nível de log em `appsettings.json`.

### 11. Próximos Testes

Após validar os básicos, teste:
- Adicionar itens a comandas
- Finalizar vendas com diferentes formas de pagamento
- Consultar vendas por período
- Verificar controle de estoque

---

**Obs**: Substitua `SEU_TOKEN_AQUI` pelo token JWT retornado no login.