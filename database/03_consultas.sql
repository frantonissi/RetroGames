-- RetroGames - Aula 1, etapa 5/6 - Consultas do catalogo
-- Execute no DataGrip depois de criar e popular as tabelas (etapas 3 e 4).
-- Estas consultas com JOIN sao exercicios SQL da aula, nao consultas da camada Repository.
USE retrogames;

-- 1) Catalogo: somente jogos ativos, plataforma e condicao; ordenado por nome.
SELECT j.co_seq_jogo AS id_jogo,
       j.no_jogo AS jogo,
       p.no_plataforma AS plataforma,
       c.ds_condicao_jogo AS condicao,
       j.vl_preco AS preco,
       j.qt_estoque AS estoque
FROM tb_jogo AS j
JOIN tb_plataforma AS p ON p.co_seq_plataforma = j.co_plataforma
JOIN tb_condicao_jogo AS c ON c.co_condicao_jogo = j.co_condicao_jogo
WHERE j.st_ativo = TRUE
ORDER BY j.no_jogo;

-- 2) Generos de um jogo informado. Altere o texto para pesquisar outro jogo.
SET @nome_jogo = 'Super Metroid';
SELECT j.no_jogo AS jogo, g.no_genero AS genero
FROM tb_jogo AS j
JOIN tb_jogo_genero AS jg ON jg.co_jogo = j.co_seq_jogo
JOIN tb_genero AS g ON g.co_seq_genero = jg.co_genero
WHERE j.no_jogo = @nome_jogo COLLATE utf8mb4_unicode_ci
ORDER BY g.no_genero;

-- 3) Jogos de um genero informado. Altere o genero para testar outro.
SET @nome_genero = 'Plataforma';
SELECT g.no_genero AS genero, j.no_jogo AS jogo
FROM tb_genero AS g
JOIN tb_jogo_genero AS jg ON jg.co_genero = g.co_seq_genero
JOIN tb_jogo AS j ON j.co_seq_jogo = jg.co_jogo
WHERE g.no_genero = @nome_genero COLLATE utf8mb4_unicode_ci
ORDER BY j.no_jogo;

-- 4) Quantidade de jogos por plataforma, INCLUINDO plataformas sem jogos.
SELECT p.no_plataforma AS plataforma,
       COUNT(j.co_seq_jogo) AS quantidade_jogos
FROM tb_plataforma AS p
LEFT JOIN tb_jogo AS j ON j.co_plataforma = p.co_seq_plataforma
GROUP BY p.co_seq_plataforma, p.no_plataforma
ORDER BY p.no_plataforma;

-- 5) Busca por parte do nome, com cinco itens por pagina.
-- Altere @busca para testar outros termos.
SET @busca = 'a';
-- Primeira pagina: itens 1 a 5.
SELECT j.co_seq_jogo AS id_jogo, j.no_jogo AS jogo, j.vl_preco AS preco
FROM tb_jogo AS j
WHERE j.no_jogo LIKE CONCAT('%', @busca, '%') COLLATE utf8mb4_unicode_ci
ORDER BY j.no_jogo, j.co_seq_jogo
LIMIT 5 OFFSET 0;
-- Segunda pagina: itens 6 a 10.
SELECT j.co_seq_jogo AS id_jogo, j.no_jogo AS jogo, j.vl_preco AS preco
FROM tb_jogo AS j
WHERE j.no_jogo LIKE CONCAT('%', @busca, '%') COLLATE utf8mb4_unicode_ci
ORDER BY j.no_jogo, j.co_seq_jogo
LIMIT 5 OFFSET 5;

