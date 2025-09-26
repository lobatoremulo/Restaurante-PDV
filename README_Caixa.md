# M�dulo de Controle de Caixa

Este m�dulo implementa um sistema completo de controle de caixa para o RestaurantePDV, permitindo o gerenciamento de abertura, fechamento e movimenta��es financeiras.

## Funcionalidades Principais

### 1. Controle de Caixa
- **Abertura de Caixa**: Permite abrir um novo caixa com valor inicial
- **Fechamento de Caixa**: Fecha o caixa atual com confer�ncia de valores
- **Status do Caixa**: Verifica se h� caixa aberto
- **Relat�rios**: Gera relat�rios detalhados por caixa e per�odos

### 2. Movimenta��es Financeiras
- **Vendas**: Registro autom�tico de vendas no caixa
- **Sangrias**: Retirada de dinheiro do caixa
- **Suprimentos**: Entrada adicional de dinheiro no caixa
- **Movimentos Personalizados**: Entradas e sa�das diversas

## Estrutura do M�dulo

### Entidades

#### Caixa
Representa uma sess�o de caixa (abertura at� fechamento):
- **DataAbertura**: Data/hora de abertura
- **DataFechamento**: Data/hora de fechamento (opcional)
- **Status**: Aberto, Fechado ou Bloqueado
- **ValorAbertura**: Valor inicial do caixa
- **ValorFechamento**: Valor final informado no fechamento
- **TotalVendas**: Soma de todas as vendas
- **TotalSangrias**: Soma de todas as sangrias
- **TotalSuprimentos**: Soma de todos os suprimentos
- **OperadorAbertura/Fechamento**: Funcion�rios respons�veis

#### MovimentoCaixa
Representa cada movimento financeiro:
- **TipoMovimento**: Venda, Sangria, Suprimento, etc.
- **Valor**: Valor do movimento
- **Descri��o**: Descri��o do movimento
- **FormaPagamento**: Forma de pagamento utilizada
- **VendaId**: Refer�ncia � venda (se aplic�vel)
- **Operador**: Funcion�rio respons�vel

### Enums

#### StatusCaixa
- **Aberto (1)**: Caixa em funcionamento
- **Fechado (2)**: Caixa finalizado
- **Bloqueado (3)**: Caixa temporariamente bloqueado

#### TipoMovimentoCaixa
- **Entrada (1)**: Entrada gen�rica
- **Saida (2)**: Sa�da gen�rica
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
Lista caixas por per�odo (Admin/Gerente)

#### `POST /api/caixa/abrir`
Abre um novo caixa
```json
{
  "valorAbertura": 100.00,
  "observacoesAbertura": "Caixa do turno da manh�",
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
Gera relat�rio completo do caixa (Admin/Gerente)

#### `GET /api/caixa/relatorio-financeiro?dataInicio=2024-01-01&dataFim=2024-01-31`
Gera relat�rio financeiro por per�odo (Admin/Gerente)

### Movimenta��es

#### `GET /api/movimentocaixa/{id}`
Busca movimento por ID (Admin/Gerente)

#### `GET /api/movimentocaixa/caixa/{caixaId}`
Lista movimentos de um caixa espec�fico

#### `GET /api/movimentocaixa/periodo?dataInicio=2024-01-01&dataFim=2024-01-31`
Lista movimentos por per�odo (Admin/Gerente)

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
  "descricao": "Sangria para dep�sito",
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

## Regras de Neg�cio

### Abertura de Caixa
1. Apenas um caixa pode estar aberto por vez
2. � obrigat�rio informar o operador respons�vel
3. Valor de abertura deve ser igual ou maior que zero
4. Se valor > 0, cria movimento autom�tico de abertura

### Fechamento de Caixa
1. Apenas caixas com status "Aberto" podem ser fechados
2. Sistema calcula saldo te�rico: abertura + vendas + suprimentos - sangrias
3. Se valor informado difere do saldo te�rico, cria movimento de diferen�a
4. Diferen�a positiva = "Sobra de caixa"
5. Diferen�a negativa = "Falta de caixa"

### Movimenta��es
1. Apenas caixas abertos aceitam novos movimentos
2. Vendas n�o podem ser registradas em duplicata no mesmo caixa
3. Valores devem ser sempre positivos
4. Sangrias requerem permiss�o de Admin/Gerente

### Seguran�a e Permiss�es
- **Admin**: Acesso total a todas as funcionalidades
- **Gerente**: Acesso total exceto exclus�es
- **UsuarioComum**: Pode abrir/fechar caixa, adicionar movimentos (exceto sangrias)

## Relat�rios Dispon�veis

### Relat�rio de Caixa Individual
- Informa��es completas do caixa
- Lista de todos os movimentos
- Resumo por tipo de movimento
- Totaliza��o por forma de pagamento
- C�lculo de diferen�as

### Relat�rio Financeiro por Per�odo
- Total de caixas abertos/fechados
- Consolida��o de vendas
- Total de sangrias e suprimentos
- An�lise por forma de pagamento
- Vendas por dia

## Instala��o e Configura��o

### 1. Executar Migration
Execute o script `Scripts/CaixaMigration.sql` no banco PostgreSQL para criar as tabelas necess�rias.

### 2. Depend�ncias
As depend�ncias j� foram registradas no `Program.cs`:
- ICaixaRepository / CaixaRepository
- IMovimentoCaixaRepository / MovimentoCaixaRepository
- ICaixaService / CaixaService
- IMovimentoCaixaService / MovimentoCaixaService

### 3. Testando a API
1. Abra um caixa: `POST /api/caixa/abrir`
2. Registre algumas movimenta��es
3. Feche o caixa: `POST /api/caixa/fechar`
4. Consulte relat�rios

## Fluxo T�pico de Uso

### In�cio do Dia
1. Funcion�rio abre o caixa com valor inicial
2. Sistema registra movimento de abertura
3. Caixa fica dispon�vel para movimenta��es

### Durante o Dia
1. Vendas s�o registradas automaticamente via integra��o
2. Sangrias s�o feitas quando necess�rio (dep�sitos)
3. Suprimentos s�o adicionados para refor�ar troco

### Final do Dia
1. Operador conta o dinheiro no caixa
2. Informa valor de fechamento no sistema
3. Sistema calcula e exibe eventuais diferen�as
4. Caixa � fechado e relat�rio � gerado

## Integra��o com Outros M�dulos

### Vendas
- Quando uma venda � finalizada, pode ser automaticamente registrada no caixa aberto
- Forma de pagamento da venda � preservada no movimento

### Funcion�rios
- Controle de quem abriu/fechou cada caixa
- Rastreabilidade de todos os movimentos por operador

### Relat�rios Gerenciais
- Dados do caixa alimentam relat�rios financeiros gerais
- An�lise de performance por per�odo e operador

## Pr�ximas Melhorias

1. **Dashboard em Tempo Real**: Visualiza��o do status atual do caixa
2. **Alertas de Diferen�a**: Notifica��es quando h� diverg�ncias significativas
3. **Backup Autom�tico**: Fechamento autom�tico em caso de sistema inativo
4. **M�ltiplos Caixas**: Suporte a v�rios caixas simult�neos (filiais)
5. **Integra��o TEF**: Controle de vendas em cart�o via TEF
6. **App Mobile**: Aplicativo para confer�ncia de caixa

## Suporte

Para d�vidas ou problemas com o m�dulo de Controle de Caixa, consulte a documenta��o da API ou entre em contato com a equipe de desenvolvimento.