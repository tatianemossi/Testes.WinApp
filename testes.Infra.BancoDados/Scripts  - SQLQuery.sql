--Criando a tabela de Disciplinas
	CREATE TABLE [dbo].[TBDisciplina] 
	(
		[Numero]   INT           IDENTITY (1, 1) NOT NULL,
		[Nome]     VARCHAR (300) NOT NULL,
		PRIMARY KEY CLUSTERED ([Numero] ASC)
	);

--Inserindo um registro na tabela
	INSERT INTO [TBDisciplina] 
	(
		[NOME]
	)
	values
	(
		'MATEMATICA'
	)

--Atualizando um registro da tabela
	UPDATE [TBDisciplina]	
		SET
			[NOME] = 'PORTUGUES'
		WHERE
			[NUMERO] = 1

--Excluindo um registro da tabela
	DELETE FROM [TBDisciplina]
		WHERE 
			[NUMERO] = 1

--Retornando todos os registros da tabela
	SELECT 
		[NUMERO], 
		[NOME]
	FROM 
		[TBDISCIPLINA]

--Retornando apenas um registro da tabela 
	SELECT 
		[NUMERO], 
		[NOME]
	FROM 
		[TBDisciplina]
	WHERE 
		[NUMERO] = 1

select count(*) from TBDisciplina