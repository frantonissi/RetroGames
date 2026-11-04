
-- RetroGames - Etapa 4 de 6: dados de exemplo
-- Execute depois de database/01_criar_tabelas.sql, em um banco com tabelas vazias.
-- Nao cadastrar usuarios: o administrador sera criado pela API em outra aula.
USE retrogames;

-- 1) Tabelas de dominio: chaves fixas (sem AUTO_INCREMENT).
INSERT INTO tb_condicao_jogo (co_condicao_jogo, ds_condicao_jogo) VALUES
(1, 'Novo'),
(2, 'Usado'),
(3, 'Recondicionado');

INSERT INTO tb_perfil (co_perfil, ds_perfil) VALUES
(1, 'Administrador'),
(2, 'Cliente');

-- 2) Seis plataformas reais e seus anos de lancamento originais.
INSERT INTO tb_plataforma (no_plataforma, no_fabricante, nu_ano_lancamento) VALUES
('NES', 'Nintendo', 1983),
('Super Nintendo', 'Nintendo', 1990),
('Mega Drive', 'Sega', 1988),
('Game Boy', 'Nintendo', 1989),
('Nintendo 64', 'Nintendo', 1996),
('PlayStation', 'Sony', 1994);

-- 3) Oito generos.
INSERT INTO tb_genero (no_genero) VALUES
('Acao'), ('Aventura'), ('Plataforma'), ('RPG'),
('Corrida'), ('Luta'), ('Puzzle'), ('Esporte');

-- 4) Vinte jogos retro reais. Referencias buscadas pelo nome,
--    sem depender da ordem dos IDs AUTO_INCREMENT.
--    A coluna ds_jogo e opcional; usamos NULL para demonstrar isso.
--    Super Metroid esta inativo; Tetris tem estoque zero.
INSERT INTO tb_jogo
(no_jogo, ds_jogo, nu_ano_lancamento, vl_preco, qt_estoque, st_ativo, co_plataforma, co_condicao_jogo)
VALUES
('Super Mario Bros.', 'Classico de plataforma do NES.', 1985, 119.90, 4, TRUE, (SELECT co_seq_plataforma FROM tb_plataforma WHERE no_plataforma='NES'), 2),
('The Legend of Zelda', 'Aventura no reino de Hyrule.', 1986, 159.90, 2, TRUE, (SELECT co_seq_plataforma FROM tb_plataforma WHERE no_plataforma='NES'), 2),
('Metroid', NULL, 1986, 149.90, 3, TRUE, (SELECT co_seq_plataforma FROM tb_plataforma WHERE no_plataforma='NES'), 3),
('Mega Man 2', 'Acao e plataformas no NES.', 1988, 139.90, 5, TRUE, (SELECT co_seq_plataforma FROM tb_plataforma WHERE no_plataforma='NES'), 2),
('Super Mario World', 'Aventura de Mario e Yoshi.', 1990, 189.90, 4, TRUE, (SELECT co_seq_plataforma FROM tb_plataforma WHERE no_plataforma='Super Nintendo'), 2),
('The Legend of Zelda: A Link to the Past', 'Aventura classica do Super Nintendo.', 1991, 229.90, 2, TRUE, (SELECT co_seq_plataforma FROM tb_plataforma WHERE no_plataforma='Super Nintendo'), 2),
('Super Metroid', 'Aventura de Samus Aran.', 1994, 249.90, 1, FALSE, (SELECT co_seq_plataforma FROM tb_plataforma WHERE no_plataforma='Super Nintendo'), 2),
('Chrono Trigger', 'RPG classico de viagem no tempo.', 1995, 329.90, 1, TRUE, (SELECT co_seq_plataforma FROM tb_plataforma WHERE no_plataforma='Super Nintendo'), 3),
('Sonic the Hedgehog', 'Estreia de Sonic no Mega Drive.', 1991, 119.90, 6, TRUE, (SELECT co_seq_plataforma FROM tb_plataforma WHERE no_plataforma='Mega Drive'), 2),
('Streets of Rage 2', 'Acao cooperativa nas ruas.', 1992, 179.90, 3, TRUE, (SELECT co_seq_plataforma FROM tb_plataforma WHERE no_plataforma='Mega Drive'), 2),
('Golden Axe', 'Fantasia e combate no Mega Drive.', 1989, 129.90, 2, TRUE, (SELECT co_seq_plataforma FROM tb_plataforma WHERE no_plataforma='Mega Drive'), 3),
('Gunstar Heroes', 'Acao com disparos e plataformas.', 1993, 219.90, 2, TRUE, (SELECT co_seq_plataforma FROM tb_plataforma WHERE no_plataforma='Mega Drive'), 2),
('Tetris', 'Quebra-cabeca de blocos.', 1989, 89.90, 0, TRUE, (SELECT co_seq_plataforma FROM tb_plataforma WHERE no_plataforma='Game Boy'), 2),
('Pokemon Red', 'RPG de captura de criaturas.', 1996, 199.90, 3, TRUE, (SELECT co_seq_plataforma FROM tb_plataforma WHERE no_plataforma='Game Boy'), 2),
('Kirby\'s Dream Land', 'Primeira aventura de Kirby.', 1992, 119.90, 4, TRUE, (SELECT co_seq_plataforma FROM tb_plataforma WHERE no_plataforma='Game Boy'), 2),
('Super Mario 64', 'Mario em tres dimensoes.', 1996, 219.90, 3, TRUE, (SELECT co_seq_plataforma FROM tb_plataforma WHERE no_plataforma='Nintendo 64'), 2),
('Mario Kart 64', 'Corridas com personagens Nintendo.', 1996, 199.90, 4, TRUE, (SELECT co_seq_plataforma FROM tb_plataforma WHERE no_plataforma='Nintendo 64'), 2),
('The Legend of Zelda: Ocarina of Time', 'Aventura tridimensional de Link.', 1998, 279.90, 2, TRUE, (SELECT co_seq_plataforma FROM tb_plataforma WHERE no_plataforma='Nintendo 64'), 3),
('Crash Bandicoot', 'Plataformas no PlayStation.', 1996, 149.90, 5, TRUE, (SELECT co_seq_plataforma FROM tb_plataforma WHERE no_plataforma='PlayStation'), 2),
('Gran Turismo', 'Simulador de corridas.', 1997, 129.90, 4, TRUE, (SELECT co_seq_plataforma FROM tb_plataforma WHERE no_plataforma='PlayStation'), 2);

-- 5) Associar cada jogo a 1 a 3 generos.
--    Os pares de nomes sao transformados nos IDs corretos por JOIN.
INSERT INTO tb_jogo_genero (co_jogo, co_genero)
SELECT j.co_seq_jogo, g.co_seq_genero
FROM (
    SELECT 'Super Mario Bros.' AS jogo, 'Plataforma' AS genero UNION ALL
    SELECT 'The Legend of Zelda', 'Acao' UNION ALL
    SELECT 'The Legend of Zelda', 'Aventura' UNION ALL
    SELECT 'Metroid', 'Acao' UNION ALL
    SELECT 'Metroid', 'Aventura' UNION ALL
    SELECT 'Mega Man 2', 'Acao' UNION ALL
    SELECT 'Mega Man 2', 'Plataforma' UNION ALL
    SELECT 'Super Mario World', 'Plataforma' UNION ALL
    SELECT 'The Legend of Zelda: A Link to the Past', 'Acao' UNION ALL
    SELECT 'The Legend of Zelda: A Link to the Past', 'Aventura' UNION ALL
    SELECT 'Super Metroid', 'Acao' UNION ALL
    SELECT 'Super Metroid', 'Aventura' UNION ALL
    SELECT 'Super Metroid', 'Plataforma' UNION ALL
    SELECT 'Chrono Trigger', 'RPG' UNION ALL
    SELECT 'Sonic the Hedgehog', 'Plataforma' UNION ALL
    SELECT 'Streets of Rage 2', 'Acao' UNION ALL
    SELECT 'Streets of Rage 2', 'Luta' UNION ALL
    SELECT 'Golden Axe', 'Acao' UNION ALL
    SELECT 'Golden Axe', 'Luta' UNION ALL
    SELECT 'Gunstar Heroes', 'Acao' UNION ALL
    SELECT 'Gunstar Heroes', 'Plataforma' UNION ALL
    SELECT 'Tetris', 'Puzzle' UNION ALL
    SELECT 'Pokemon Red', 'RPG' UNION ALL
    SELECT 'Pokemon Red', 'Aventura' UNION ALL
    SELECT 'Kirby\'s Dream Land', 'Plataforma' UNION ALL
    SELECT 'Super Mario 64', 'Plataforma' UNION ALL
    SELECT 'Super Mario 64', 'Aventura' UNION ALL
    SELECT 'Mario Kart 64', 'Corrida' UNION ALL
    SELECT 'The Legend of Zelda: Ocarina of Time', 'Acao' UNION ALL
    SELECT 'The Legend of Zelda: Ocarina of Time', 'Aventura' UNION ALL
    SELECT 'Crash Bandicoot', 'Plataforma' UNION ALL
    SELECT 'Gran Turismo', 'Corrida' UNION ALL
    SELECT 'Gran Turismo', 'Esporte'
) AS associacoes
JOIN tb_jogo AS j ON j.no_jogo = associacoes.jogo
JOIN tb_genero AS g ON g.no_genero = associacoes.genero;

-- 6) Conferencias exigidas pelo roteiro.
SELECT (SELECT COUNT(*) FROM tb_plataforma) AS plataformas,
       (SELECT COUNT(*) FROM tb_genero) AS generos,
       (SELECT COUNT(*) FROM tb_jogo) AS jogos,
       (SELECT COUNT(*) FROM tb_usuario) AS usuarios;

-- Deve retornar zero linhas: todos os jogos possuem genero.
SELECT j.co_seq_jogo, j.no_jogo
FROM tb_jogo AS j
LEFT JOIN tb_jogo_genero AS jg ON jg.co_jogo = j.co_seq_jogo
WHERE jg.co_jogo IS NULL;

-- Deve mostrar os exemplos de jogo inativo e de estoque zero.
SELECT no_jogo, st_ativo, qt_estoque
FROM tb_jogo
WHERE st_ativo = FALSE OR qt_estoque = 0;

-- Deve mostrar entre 1 e 3 generos para cada um dos 20 jogos.
SELECT j.no_jogo, COUNT(*) AS quantidade_generos
FROM tb_jogo j
JOIN tb_jogo_genero jg ON jg.co_jogo = j.co_seq_jogo
GROUP BY j.co_seq_jogo, j.no_jogo
ORDER BY j.no_jogo;
