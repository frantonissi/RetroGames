-- RetroGames - Aula 1, etapa 5/6 - Testes de integridade
-- IMPORTANTE: execute CADA TESTE SEPARADAMENTE, no DataGrip.
-- Descomente apenas o bloco escolhido, execute START TRANSACTION,
-- o comando de teste e ROLLBACK. Os erros esperados sao parte da atividade.
-- Se o DataGrip parar ao encontrar um erro, execute ROLLBACK isoladamente.
-- Nao execute todos os testes de uma vez. Nunca use COMMIT nestes testes.
USE retrogames;

-- TESTE 1 - FK inexistente: resultado esperado = erro MySQL 1452.
-- Nenhum registro deve ser adicionado.
 START TRANSACTION;
 INSERT INTO tb_jogo
 (no_jogo, ds_jogo, nu_ano_lancamento, vl_preco, qt_estoque, st_ativo, co_plataforma, co_condicao_jogo)
 VALUES ('Teste FK inexistente', NULL, 1990, 10.00, 1, TRUE, -1, 2);
 ROLLBACK;
-- Resultado obtido: _Cannot add or update a child row: a foreign key constraint fails_

use retrogames;

-- TESTE 2 - Plataforma com nome ja cadastrado: resultado esperado = erro MySQL 1062 (UNIQUE).
-- A etapa 4 cadastrou a plataforma 'NES'.
 START TRANSACTION;
 INSERT INTO tb_plataforma (no_plataforma, no_fabricante, nu_ano_lancamento)
 VALUES ('NES', 'Nintendo', 1983);
 ROLLBACK;
-- Resultado obtido: _ Duplicate entry 'NES' for key 'tb_plataforma.no_plataforma'_

use retrogames;
-- TESTE 3 - Excluir plataforma que possui jogos: resultado esperado = erro MySQL 1451 (FK).
-- A etapa 4 cadastrou jogos do NES.
 START TRANSACTION;
 DELETE FROM tb_plataforma WHERE no_plataforma = 'NES';
 ROLLBACK;
-- Resultado obtido: _Cannot delete or update a parent row: a foreign key constraint fails_

use retrogames;

-- TESTE 4 - Nome do jogo nulo: resultado esperado = erro MySQL 1048 (NOT NULL).
 START TRANSACTION;
 INSERT INTO tb_jogo
 (no_jogo, ds_jogo, nu_ano_lancamento, vl_preco, qt_estoque, st_ativo, co_plataforma, co_condicao_jogo)
 VALUES (NULL, 'Teste nome nulo', 1990, 10.00, 1, TRUE,
         (SELECT co_seq_plataforma FROM tb_plataforma WHERE no_plataforma = 'NES'), 2);
 ROLLBACK;
-- Resultado obtido: _ Column 'no_jogo' cannot be null_

use retrogames;
-- TESTE 5 - Preco igual a zero: resultado esperado = ACEITO pelo banco.
-- A regra preco > 0 sera verificada na camada Service (RN06), nao na tabela.
-- Este e o UNICO teste que deve inserir temporariamente um registro.
 START TRANSACTION;
 INSERT INTO tb_jogo
 (no_jogo, ds_jogo, nu_ano_lancamento, vl_preco, qt_estoque, st_ativo, co_plataforma, co_condicao_jogo)
 VALUES ('Teste preco zero', NULL, 1990, 0.00, 1, TRUE,
         (SELECT co_seq_plataforma FROM tb_plataforma WHERE no_plataforma = 'NES'), 2);
 SELECT co_seq_jogo, no_jogo, vl_preco FROM tb_jogo WHERE no_jogo = 'Teste preco zero';
 ROLLBACK;
-- Resultado obtido: _Deu certo pois sera tratado na camada service_

-- VERIFICACAO FINAL (pode executar diretamente): deve retornar zero.
SELECT COUNT(*) AS testes_restantes
FROM tb_jogo
WHERE no_jogo IN ('Teste FK inexistente', 'Teste preco zero');
