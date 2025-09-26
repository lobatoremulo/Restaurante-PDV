# Módulo de Controle de Caixa

Este módulo implementa um sistema completo de controle de caixa para o RestaurantePDV, permitindo o gerenciamento de abertura, fechamento e movimentações financeiras.

## Funcionalidades Principais

### 1. Controle de Caixa
- **Abertura de Caixa**: Permite abrir um novo caixa com valor inicial
- **Fechamento de Caixa**: Fecha o caixa atual com conferência de valores
- **Status do Caixa**: Verifica se há caixa aberto
- **Relatórios**: Gera relatórios detalhados por caixa e períodos

### 2. Movimentações Financeiras
- **Vendas**: Registro automático de vendas no caixa
- **Sangrias**: Retirada de dinheiro do caixa
- **Suprimentos**: Entrada adicional de dinheiro no caixa
- **Movimentos Personalizados**: Entradas e saídas diversas

## Estrutura do Módulo

### Entidades

#### Caixa
Representa uma sessão de caixa (abertura até fechamento):
- **DataAbertura**: Data/hora de abertura
- **DataFechamento**: Data/hora de fechamento (opcional)
- **Status**: Aberto, Fechado ou Bloqueado
- **ValorAbertura**: Valor inicial do caixa
- **ValorFechamento**: Valor final informado no fechamento
- **TotalVendas**: Soma de todas as vendas
- **TotalSangrias**: Soma de todas as sangrias
- **TotalSuprimentos**: Soma de todos os suprimentos
- **OperadorAbertura/Fechamento**: Funcionários responsáveis

#### MovimentoCaixa
Representa cada movimento financeiro:
- **TipoMovimento**: Venda, Sangria, Suprimento, etc.
- **Valor**: Valor do movimento
- **Descrição**: Descrição do movimento
- **FormaPagamento**: Forma de pagamento utilizada
- **VendaId**: Referência à venda (se aplicável)
- **Operador**: Funcionário responsável

### Enums

#### StatusCaixa
- **Aberto (1)**: Caixa em funcionamento
- **Fechado (2)**: Caixa finalizado
- **Bloqueado (3)**: Caixa temporariamente bloqueado

#### TipoMovimentoCaixa
- **Entrada (1)**: Entrada genérica
- **Saida (2)**: Saída genérica
- **Venda (3)**: Venda realizada
- **Sangria (4)**: Retirada de dinheiro
- **Suprimento (5)**: Entrada adicional de dinheiro
- **Abertura (6)**: Movimento de abertura
- **Fechamento (7)**: Movimento de fechamento

## API Endpoints

### Controle de Caixa

#### `GET /api/caixa`
Lista todos os caixas (Admin/Gerente)

#### `GET /api/caixa/aberto`
Retorna o caixa atualmente aberto

#### `GET /api/caixa/status`
Verifica o status do caixa (aberto/fechado)

#### `GET /api/caixa/{id}`
Busca caixa por ID

#### `GET /api/caixa/periodo?dataInicio=2024-01-01&dataFim=2024-01-31`
Lista caixas por período (Admin/Gerente)

#### `POST /api/caixa/abrir`
Abre um novo caixa
```json
{
  "valorAbertura": 100.00,
  "observacoesAbertura": "Caixa do turno da manhã",
  "operadorAberturaId": 1
}
```

#### `POST /api/caixa/fechar`
Fecha o caixa atual
```json
{
  "caixaId": 1,
  "valorFechamento": 850.50,
  "observacoesFechamento": "Fechamento normal",
  "operadorFechamentoId": 1
}
```

#### `GET /api/caixa/{id}/relatorio`
Gera relatório completo do caixa (Admin/Gerente)

#### `GET /api/caixa/relatorio-financeiro?dataInicio=2024-01-01&dataFim=2024-01-31`
Gera relatório financeiro por período (Admin/Gerente)

### Movimentações

#### `GET /api/movimentocaixa/{id}`
Busca movimento por ID (Admin/Gerente)

#### `GET /api/movimentocaixa/caixa/{caixaId}`
Lista movimentos de um caixa específico

#### `GET /api/movimentocaixa/periodo?dataInicio=2024-01-01&dataFim=2024-01-31`
Lista movimentos por período (Admin/Gerente)

#### `POST /api/movimentocaixa`
Adiciona movimento personalizado
```json
{
  "caixaId": 1,
  "tipoMovimento": 1,
  "valor": 50.00,
  "descricao": "Troco inicial",
  "observacoes": "Adicional para troco",
  "formaPagamento": 1,
  "operadorId": 1
}
```

#### `POST /api/movimentocaixa/registrar-venda`
Registra uma venda no caixa
```json
{
  "vendaId": 123,
  "caixaId": 1,
  "operadorId": 1
}
```

#### `POST /api/movimentocaixa/sangria`
Registra uma sangria (Admin/Gerente)
```json
{
  "caixaId": 1,
  "valor": 200.00,
  "descricao": "Sangria para depósito",
  "observacoes": "Banco do Brasil",
  "operadorId": 1
}
```

#### `POST /api/movimentocaixa/suprimento`
Registra um suprimento
```json
{
  "caixaId": 1,
  "valor": 100.00,
  "descricao": "Suprimento para troco",
  "observacoes": "Troco adicional",
  "operadorId": 1
}
```

## Regras de Negócio

### Abertura de Caixa
1. Apenas um caixa pode estar aberto por vez
2. É obrigatório informar o operador responsável
3. Valor de abertura deve ser igual ou maior que zero
4. Se valor > 0, cria movimento automático de abertura

### Fechamento de Caixa
1. Apenas caixas com status "Aberto" podem ser fechados
2. Sistema calcula saldo teórico: abertura + vendas + suprimentos - sangrias
3. Se valor informado difere do saldo teórico, cria movimento de diferença
4. Diferença positiva = "Sobra de caixa"
5. Diferença negativa = "Falta de caixa"

### Movimentações
1. Apenas caixas abertos aceitam novos movimentos
2. Vendas não podem ser registradas em duplicata no mesmo caixa
3. Valores devem ser sempre positivos
4. Sangrias requerem permissão de Admin/Gerente

### Segurança e Permissões
- **Admin**: Acesso total a todas as funcionalidades
- **Gerente**: Acesso total exceto exclusões
- **UsuarioComum**: Pode abrir/fechar caixa, adicionar movimentos (exceto sangrias)

## Relatórios Disponíveis

### Relatório de Caixa Individual
- Informações completas do caixa
- Lista de todos os movimentos
- Resumo por tipo de movimento
- Totalização por forma de pagamento
- Cálculo de diferenças

### Relatório Financeiro por Período
- Total de caixas abertos/fechados
- Consolidação de vendas
- Total de sangrias e suprimentos
- Análise por forma de pagamento
- Vendas por dia

## Instalação e Configuração

### 1. Executar Migration
Execute o script `Scripts/CaixaMigration.sql` no banco PostgreSQL para criar as tabelas necessárias.

### 2. Dependências
As dependências já foram registradas no `Program.cs`:
- ICaixaRepository / CaixaRepository
- IMovimentoCaixaRepository / MovimentoCaixaRepository
- ICaixaService / CaixaService
- IMovimentoCaixaService / MovimentoCaixaService

### 3. Testando a API
1. Abra um caixa: `POST /api/caixa/abrir`
2. Registre algumas movimentações
3. Feche o caixa: `POST /api/caixa/fechar`
4. Consulte relatórios

## Fluxo Típico de Uso

### Início do Dia
1. Funcionário abre o caixa com valor inicial
2. Sistema registra movimento de abertura
3. Caixa fica disponível para movimentações

### Durante o Dia
1. Vendas são registradas automaticamente via integração
2. Sangrias são feitas quando necessário (depósitos)
3. Suprimentos são adicionados para reforçar troco

### Final do Dia
1. Operador conta o dinheiro no caixa
2. Informa valor de fechamento no sistema
3. Sistema calcula e exibe eventuais diferenças
4. Caixa é fechado e relatório é gerado

## Integração com Outros Módulos

### Vendas
- Quando uma venda é finalizada, pode ser automaticamente registrada no caixa aberto
- Forma de pagamento da venda é preservada no movimento

### Funcionários
- Controle de quem abriu/fechou cada caixa
- Rastreabilidade de todos os movimentos por operador

### Relatórios Gerenciais
- Dados do caixa alimentam relatórios financeiros gerais
- Análise de performance por período e operador

## Próximas Melhorias

1. **Dashboard em Tempo Real**: Visualização do status atual do caixa
2. **Alertas de Diferença**: Notificações quando há divergências significativas
3. **Backup Automático**: Fechamento automático em caso de sistema inativo
4. **Múltiplos Caixas**: Suporte a vários caixas simultâneos (filiais)
5. **Integração TEF**: Controle de vendas em cartão via TEF
6. **App Mobile**: Aplicativo para conferência de caixa

## Suporte

Para dúvidas ou problemas com o módulo de Controle de Caixa, consulte a documentação da API ou entre em contato com a equipe de desenvolvimento.