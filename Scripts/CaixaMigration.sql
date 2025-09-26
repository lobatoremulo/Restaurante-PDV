-- Migration para adicionar as tabelas do Controle de Caixa
-- Execute este script no banco de dados PostgreSQL

-- Tabela Caixas
CREATE TABLE "Caixas" (
    "Id" SERIAL PRIMARY KEY,
    "DataAbertura" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    "DataFechamento" TIMESTAMP WITH TIME ZONE NULL,
    "Status" INTEGER NOT NULL DEFAULT 1, -- 1: Aberto, 2: Fechado, 3: Bloqueado
    "ValorAbertura" DECIMAL(18,2) NOT NULL DEFAULT 0,
    "ValorFechamento" DECIMAL(18,2) NOT NULL DEFAULT 0,
    "TotalVendas" DECIMAL(18,2) NOT NULL DEFAULT 0,
    "TotalSangrias" DECIMAL(18,2) NOT NULL DEFAULT 0,
    "TotalSuprimentos" DECIMAL(18,2) NOT NULL DEFAULT 0,
    "ObservacoesAbertura" VARCHAR(500) NULL,
    "ObservacoesFechamento" VARCHAR(500) NULL,
    "OperadorAberturaId" INTEGER NOT NULL,
    "OperadorFechamentoId" INTEGER NULL,
    "CriadoEm" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    "AtualizadoEm" TIMESTAMP WITH TIME ZONE NULL,
    "Ativo" BOOLEAN NOT NULL DEFAULT TRUE,
    
    CONSTRAINT "FK_Caixas_Funcionarios_OperadorAberturaId" 
        FOREIGN KEY ("OperadorAberturaId") REFERENCES "Funcionarios"("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_Caixas_Funcionarios_OperadorFechamentoId" 
        FOREIGN KEY ("OperadorFechamentoId") REFERENCES "Funcionarios"("Id") ON DELETE RESTRICT
);

-- Tabela MovimentosCaixa
CREATE TABLE "MovimentosCaixa" (
    "Id" SERIAL PRIMARY KEY,
    "CaixaId" INTEGER NOT NULL,
    "DataMovimento" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    "TipoMovimento" INTEGER NOT NULL, -- 1: Entrada, 2: Saida, 3: Venda, 4: Sangria, 5: Suprimento, 6: Abertura, 7: Fechamento
    "Valor" DECIMAL(18,2) NOT NULL,
    "Descricao" VARCHAR(200) NOT NULL,
    "Observacoes" VARCHAR(500) NULL,
    "FormaPagamento" INTEGER NULL, -- 1: Dinheiro, 2: CartaoCredito, 3: CartaoDebito, 4: Pix, 5: Fiado
    "VendaId" INTEGER NULL,
    "OperadorId" INTEGER NOT NULL,
    "NumeroDocumento" VARCHAR(100) NULL,
    "CriadoEm" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    "AtualizadoEm" TIMESTAMP WITH TIME ZONE NULL,
    "Ativo" BOOLEAN NOT NULL DEFAULT TRUE,
    
    CONSTRAINT "FK_MovimentosCaixa_Caixas_CaixaId" 
        FOREIGN KEY ("CaixaId") REFERENCES "Caixas"("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_MovimentosCaixa_Vendas_VendaId" 
        FOREIGN KEY ("VendaId") REFERENCES "Vendas"("Id") ON DELETE SET NULL,
    CONSTRAINT "FK_MovimentosCaixa_Funcionarios_OperadorId" 
        FOREIGN KEY ("OperadorId") REFERENCES "Funcionarios"("Id") ON DELETE RESTRICT
);

-- Índices para melhorar performance
CREATE INDEX "IX_Caixas_Status" ON "Caixas"("Status");
CREATE INDEX "IX_Caixas_DataAbertura" ON "Caixas"("DataAbertura");
CREATE INDEX "IX_Caixas_OperadorAberturaId" ON "Caixas"("OperadorAberturaId");
CREATE INDEX "IX_Caixas_OperadorFechamentoId" ON "Caixas"("OperadorFechamentoId");

CREATE INDEX "IX_MovimentosCaixa_CaixaId" ON "MovimentosCaixa"("CaixaId");
CREATE INDEX "IX_MovimentosCaixa_DataMovimento" ON "MovimentosCaixa"("DataMovimento");
CREATE INDEX "IX_MovimentosCaixa_TipoMovimento" ON "MovimentosCaixa"("TipoMovimento");
CREATE INDEX "IX_MovimentosCaixa_VendaId" ON "MovimentosCaixa"("VendaId");
CREATE INDEX "IX_MovimentosCaixa_OperadorId" ON "MovimentosCaixa"("OperadorId");

-- Comentários nas tabelas
COMMENT ON TABLE "Caixas" IS 'Tabela para controle de abertura e fechamento de caixa';
COMMENT ON TABLE "MovimentosCaixa" IS 'Tabela para registro de todos os movimentos financeiros do caixa';

-- Comentários nas colunas principais
COMMENT ON COLUMN "Caixas"."Status" IS '1: Aberto, 2: Fechado, 3: Bloqueado';
COMMENT ON COLUMN "MovimentosCaixa"."TipoMovimento" IS '1: Entrada, 2: Saida, 3: Venda, 4: Sangria, 5: Suprimento, 6: Abertura, 7: Fechamento';
COMMENT ON COLUMN "MovimentosCaixa"."FormaPagamento" IS '1: Dinheiro, 2: CartaoCredito, 3: CartaoDebito, 4: Pix, 5: Fiado';