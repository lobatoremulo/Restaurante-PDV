# Sistema PDV para Restaurante - API

Uma API completa em .NET 9 com C# seguindo Clean Architecture e conceitos básicos de DDD para sistema PDV de restaurante.

## 🏗️ Arquitetura

O projeto está estruturado em camadas seguindo Clean Architecture:

```
RestaurantePDV/
├── RestaurantePDV.Domain/          # Camada de Domínio
│   ├── Entities/                   # Entidades de negócio
│   ├── Enums/                      # Enumeradores
│   └── Common/                     # Classes base
├── RestaurantePDV.Application/     # Camada de Aplicação
│   ├── DTOs/                       # Data Transfer Objects
│   ├── Interfaces/                 # Interfaces dos repositórios
│   └── Services/                   # Serviços de aplicação
├── RestaurantePDV.Infrastructure/  # Camada de Infraestrutura
│   ├── Data/                       # Context do Entity Framework
│   └── Repositories/               # Implementação dos repositórios
└── RestaurantePDV.API/            # Camada de Apresentação
    └── Controllers/                # Controllers REST
```

## 🚀 Funcionalidades

### Módulos Implementados

- ✅ **Cadastro de Clientes** - CRUD completo com validações
- ✅ **Cadastro de Produtos** - Com controle de estoque e fotos
- ✅ **Cadastro de Insumos** - Com ficha técnica
- ✅ **Cadastro de Fornecedores** - Gestão de fornecedores
- ✅ **Controle de Comandas** - Abrir, fechar, cancelar comandas
- ✅ **Vendas de Balcão** - Sistema completo de vendas
- ✅ **Controle de Estoque** - Movimentações automáticas
- ✅ **Contas a Pagar/Receber** - Gestão financeira
- ✅ **Autenticação JWT** - Sistema de login seguro

### Tecnologias Utilizadas

- **.NET 9** - Framework principal
- **C#** - Linguagem de programação
- **Entity Framework Core** - ORM para PostgreSQL
- **PostgreSQL** - Banco de dados
- **JWT Bearer** - Autenticação
- **Swagger/OpenAPI** - Documentação da API
- **Clean Architecture** - Padrão arquitetural
- **Repository Pattern** - Padrão de repositório
- **Data Annotations** - Validações

## 🛠️ Configuração e Execução

### Pré-requisitos

- .NET 9 SDK
- PostgreSQL
- Visual Studio Code ou Visual Studio

### 1. Clone o repositório
```bash
git clone <url-do-repositorio>
cd RestaurantePDV
```

### 2. Configure o PostgreSQL

No arquivo `RestaurantePDV.API/appsettings.json`, configure a string de conexão:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=RestaurantePDV;Username=seu_usuario;Password=sua_senha"
  }
}
```

### 3. Execute as migrations

```bash
dotnet ef database update --project RestaurantePDV.Infrastructure --startup-project RestaurantePDV.API
```

### 4. Execute a aplicação

```bash
dotnet run --project RestaurantePDV.API
```

A API estará disponível em:
- HTTP: http://localhost:5000
- HTTPS: https://localhost:5001
- Swagger UI: https://localhost:5001/swagger

## 🔐 Autenticação

### Login de Teste

Usuários pré-configurados para teste:

| Username | Password | Role |
|----------|----------|------|
| admin | 123456 | Administrator |
| operador | 123456 | Operator |
| garcom | 123456 | Waiter |

### Endpoints de Autenticação

```http
POST /api/auth/login
Content-Type: application/json

{
  "username": "admin",
  "password": "123456"
}
```

Resposta:
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiration": "2025-09-25T16:42:00Z",
  "username": "admin"
}
```

## 📊 Principais Endpoints

### Clientes

- `GET /api/clientes` - Listar clientes ativos
- `GET /api/clientes/{id}` - Buscar cliente por ID
- `GET /api/clientes/cpf-cnpj/{cpfCnpj}` - Buscar por CPF/CNPJ
- `POST /api/clientes` - Criar novo cliente
- `PUT /api/clientes/{id}` - Atualizar cliente
- `DELETE /api/clientes/{id}` - Desativar cliente

### Comandas

- `GET /api/comandas` - Listar comandas ativas
- `GET /api/comandas/{id}` - Buscar comanda por ID
- `POST /api/comandas` - Abrir nova comanda
- `PUT /api/comandas/{id}/fechar` - Fechar comanda
- `PUT /api/comandas/{id}/cancelar` - Cancelar comanda
- `POST /api/comandas/{id}/itens` - Adicionar item à comanda

### Vendas

- `GET /api/vendas/{id}` - Buscar venda por ID
- `GET /api/vendas/periodo?dataInicio=2025-09-01&dataFim=2025-09-30` - Vendas por período
- `POST /api/vendas` - Criar nova venda
- `PUT /api/vendas/{id}/finalizar` - Finalizar venda
- `PUT /api/vendas/{id}/cancelar` - Cancelar venda

## 📝 Exemplo de Uso

### 1. Fazer Login
```bash
curl -X POST "https://localhost:5001/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{"username": "admin", "password": "123456"}'
```

### 2. Criar Cliente
```bash
curl -X POST "https://localhost:5001/api/clientes" \
  -H "Authorization: Bearer {seu_token}" \
  -H "Content-Type: application/json" \
  -d '{
    "nome": "João Silva",
    "cpfCnpj": "12345678901",
    "telefone": "(11) 99999-9999",
    "email": "joao@exemplo.com"
  }'
```

### 3. Abrir Comanda
```bash
curl -X POST "https://localhost:5001/api/comandas" \
  -H "Authorization: Bearer {seu_token}" \
  -H "Content-Type: application/json" \
  -d '{
    "clienteId": 1,
    "mesa": "Mesa 05",
    "observacoes": "Cliente preferencial"
  }'
```

### 4. Adicionar Item à Comanda
```bash
curl -X POST "https://localhost:5001/api/comandas/1/itens" \
  -H "Authorization: Bearer {seu_token}" \
  -H "Content-Type: application/json" \
  -d '{
    "produtoId": 1,
    "quantidade": 2,
    "observacoes": "Sem cebola"
  }'
```

## 📋 Estrutura do Banco de Dados

### Principais Entidades

- **Clientes**: Cadastro de clientes com limite de crédito
- **Produtos**: Catálogo de produtos com controle de estoque
- **Insumos**: Matérias-primas com controle de validade
- **Fornecedores**: Cadastro de fornecedores
- **Comandas**: Controle de mesas e pedidos
- **Vendas**: Registro de vendas com múltiplas formas de pagamento
- **MovimentoEstoque**: Controle de entrada e saída de estoque
- **ContasPagar/Receber**: Controle financeiro

### Relacionamentos

- Cliente → N Vendas
- Cliente → N Comandas  
- Produto → N ItensVenda
- Produto → N ItensComanda
- Comanda → N ItensComanda
- Venda → N ItensVenda
- Fornecedor → N Insumos
- Produto → N FichasTecnicas ← N Insumos

## 🎯 Próximos Passos

- [ ] Implementar upload de fotos de produtos
- [ ] Sistema completo de usuários e permissões
- [ ] Relatórios de vendas e estoque
- [ ] Integração com impressoras térmicas
- [ ] Dashboard em tempo real
- [ ] Módulo de delivery
- [ ] Sistema de promoções e descontos
- [ ] Backup automático de dados

## 🤝 Contribuição

1. Fork o projeto
2. Crie uma branch para sua feature
3. Commit suas mudanças
4. Push para a branch
5. Abra um Pull Request

## 📄 Licença

Este projeto está sob a licença MIT. Veja o arquivo [LICENSE](LICENSE) para mais detalhes.

---

**Desenvolvido com ❤️ para facilitar a gestão de restaurantes e estabelecimentos alimentícios.**