using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Testes.Dominio.ModuloDisciplina;
using Testes.Dominio.ModuloMateria;
using Testes.Dominio.ModuloQuestao;

namespace testes.Infra.BancoDados.ModuloQuestao
{
    public class RepositorioQuestaoEmBancoDados : IRepositorioQuestaoObjetiva
    {
        private const string enderecoBanco =
               "Data Source=(LocalDB)\\MSSqlLocalDB;" +
               "Initial Catalog=testesDb;" +
               "Integrated Security=True;" +
               "Pooling=False";

        #region Sql Queries
        private const string sqlInserir =
            @"INSERT INTO [TBQUESTAO] 
                (
                    ENUNCIADO,
                    BIMESTRE,
                    DISCIPLINA_NUMERO,
                    MATERIA_NUMERO
                )
	            VALUES
                (
                    @ENUNCIADO,
                    @BIMESTRE,
                    @DISCIPLINA_NUMERO,
                    @MATERIA_NUMERO
                );SELECT SCOPE_IDENTITY();";

        private const string sqlEditar =
            @"UPDATE [TBQUESTAO]	
		        SET
			        ENUNCIADO = @ENUNCIADO,
                    BIMESTRE = @BIMESTRE,
                    DISCIPLINA_NUMERO = @DISCIPLINA_NUMERO,
                    MATERIA_NUMERO = @MATERIA_NUMERO
		        WHERE
			        NUMERO = @NUMERO";

        private const string sqlExcluir =
            @"DELETE FROM [TBQUESTAO]
		        WHERE
			        [NUMERO] = @NUMERO";

        private const string sqlSelecionarTodos =
            @"SELECT
	            Q.NUMERO,
	            Q.ENUNCIADO,
                Q.BIMESTRE,
	            M.NOME AS MATERIA_NOME,
				M.NUMERO AS MATERIA_NUMERO,
	            D.NOME AS DISCIPLINA_NOME,
				D.NUMERO AS DISCIPLINA_NUMERO
            FROM 
	            TBQUESTAO AS Q 
            INNER JOIN TBDISCIPLINA AS D ON Q.DISCIPLINA_NUMERO = D.NUMERO
	        INNER JOIN TBMATERIA AS M ON Q.MATERIA_NUMERO = M.NUMERO";

        private const string sqlSelecionarPorNumero =
            @"SELECT
	            Q.NUMERO,
	            Q.ENUNCIADO,
                Q.BIMESTRE,
	            M.NOME AS MATERIA_NOME,
				M.NUMERO AS MATERIA_NUMERO,
	            D.NOME AS DISCIPLINA_NOME,
				D.NUMERO AS DISCIPLINA_NUMERO
            FROM 
	            TBQUESTAO AS Q 
            INNER JOIN TBDISCIPLINA AS D ON Q.DISCIPLINA_NUMERO = D.NUMERO
	        INNER JOIN TBMATERIA AS M ON Q.MATERIA_NUMERO = M.NUMERO
            WHERE
                    Q.NUMERO = @NUMERO";

        #endregion

        public ValidationResult Inserir(QuestaoObjetiva questao)
        {
            var validador = new ValidadorQuestaoObjetiva();

            var resultadoValidacao = validador.Validate(questao);

            if (resultadoValidacao.IsValid == false)
                return resultadoValidacao;

            SqlConnection conexaoComBanco = new SqlConnection(enderecoBanco);

            SqlCommand comandoInsercao = new SqlCommand(sqlInserir, conexaoComBanco);

            ConfigurarParametrosQuestao(questao, comandoInsercao);

            conexaoComBanco.Open();
            var id = comandoInsercao.ExecuteScalar();
            questao.Numero = Convert.ToInt32(id);

            conexaoComBanco.Close();

            return new ValidationResult();
        }

        public ValidationResult Editar(QuestaoObjetiva questao)
        {
            var validador = new ValidadorQuestaoObjetiva();

            var resultadoValidacao = validador.Validate(questao);

            if (resultadoValidacao.IsValid == false)
                return resultadoValidacao;

            SqlConnection conexaoComBanco = new SqlConnection(enderecoBanco);

            SqlCommand comandoEdicao = new SqlCommand(sqlEditar, conexaoComBanco);

            comandoEdicao.Parameters.AddWithValue("NUMERO", questao.Numero);

            ConfigurarParametrosQuestao(questao, comandoEdicao);

            conexaoComBanco.Open();
            comandoEdicao.ExecuteNonQuery();
            conexaoComBanco.Close();

            return resultadoValidacao;
        }

        public ValidationResult Excluir(QuestaoObjetiva questao)
        {
            SqlConnection conexaoComBanco = new SqlConnection(enderecoBanco);

            SqlCommand comandoExclusao = new SqlCommand(sqlExcluir, conexaoComBanco);

            comandoExclusao.Parameters.AddWithValue("NUMERO", questao.Numero);

            conexaoComBanco.Open();
            int numeroRegistrosExcluidos = comandoExclusao.ExecuteNonQuery();

            var resultadoValidacao = new ValidationResult();

            if (numeroRegistrosExcluidos == 0)
                resultadoValidacao.Errors.Add(new ValidationFailure("", "Não foi possível remover o registro"));

            conexaoComBanco.Close();

            return resultadoValidacao;
        }

        public List<QuestaoObjetiva> SelecionarTodos()
        {
            SqlConnection conexaoComBanco = new SqlConnection(enderecoBanco);

            SqlCommand comandoSelecao = new SqlCommand(sqlSelecionarTodos, conexaoComBanco);

            conexaoComBanco.Open();
            SqlDataReader leitorQuestao = comandoSelecao.ExecuteReader();

            List<QuestaoObjetiva> questoes = new List<QuestaoObjetiva>();

            while (leitorQuestao.Read())
            {
                var questao = ConverterParaQuestao(leitorQuestao);

                questoes.Add(questao);
            }

            conexaoComBanco.Close();

            return questoes;
        }

        public QuestaoObjetiva SelecionarPorNumero(int numero)
        {
            SqlConnection conexaoComBanco = new SqlConnection(enderecoBanco);

            SqlCommand comandoSelecao = new SqlCommand(sqlSelecionarPorNumero, conexaoComBanco);

            comandoSelecao.Parameters.AddWithValue("NUMERO", numero);

            conexaoComBanco.Open();
            SqlDataReader leitorQuestao = comandoSelecao.ExecuteReader();

            QuestaoObjetiva questao = null;
            if (leitorQuestao.Read())
                questao = ConverterParaQuestao(leitorQuestao);

            conexaoComBanco.Close();

            return questao;
        }

        private static QuestaoObjetiva ConverterParaQuestao(SqlDataReader leitorQuestao)
        {
            var numero = Convert.ToInt32(leitorQuestao["NUMERO"]);
            var enunciado = Convert.ToString(leitorQuestao["ENUNCIADO"]);
            var bimestre = Convert.ToString(leitorQuestao["BIMESTRE"]);

            var numeroDisciplina = Convert.ToInt32(leitorQuestao["DISCIPLINA_NUMERO"]);
            var nomeDisciplina = Convert.ToString(leitorQuestao["DISCIPLINA_NOME"]);

            var numeroMateria = Convert.ToInt32(leitorQuestao["MATERIA_NUMERO"]);
            var nomeMateria = Convert.ToString(leitorQuestao["MATERIA_NOME"]);


            var questao = new QuestaoObjetiva
            {
                Numero = numero,
                Enunciado = enunciado,
                Bimestre = bimestre,
                NumeroDisciplina = numeroDisciplina,
                NumeroMateria = numeroMateria,

                Disciplina = new Disciplina
                {
                    Numero = numeroDisciplina,
                    Nome = nomeDisciplina
                },

                Materia = new Materia
                {
                    Numero = numeroMateria,
                    Nome = nomeMateria
                }
            };

            return questao;
        }

        private void ConfigurarParametrosQuestao(QuestaoObjetiva questao, SqlCommand comando)
        {
            comando.Parameters.AddWithValue("ENUNCIADO", questao.Enunciado);
            comando.Parameters.AddWithValue("BIMESTRE", questao.Bimestre);
            comando.Parameters.AddWithValue("DISCIPLINA_NUMERO", questao.Disciplina.Numero);
            comando.Parameters.AddWithValue("MATERIA_NUMERO", questao.Materia.Numero);
        }

    }
}
