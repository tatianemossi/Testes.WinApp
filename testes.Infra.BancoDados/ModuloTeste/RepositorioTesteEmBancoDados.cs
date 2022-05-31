using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Testes.Dominio.ModuloDisciplina;
using Testes.Dominio.ModuloMateria;
using Testes.Dominio.ModuloQuestao;
using Testes.Dominio.ModuloTeste;

namespace testes.Infra.BancoDados.ModuloTeste
{
    public class RepositorioTesteEmBancoDados : IRepositorioTeste
    {
        private const string enderecoBanco =
               "Data Source=(LocalDB)\\MSSqlLocalDB;" +
               "Initial Catalog=testesDb;" +
               "Integrated Security=True;" +
               "Pooling=False";

        #region Queries
        private const string sqlInserir =
            @"INSERT INTO [TBTESTE] 
                (
                    TITULO,
                    DATA,
                    DISCIPLINA_NUMERO,
                    MATERIA_NUMERO,
                    RECUPERACAO,
                    QUANTIDADE_QUESTOES
                )
	            VALUES
                (
                    @TITULO,
                    @DATA,
                    @DISCIPLINA_NUMERO,
                    @MATERIA_NUMERO,
                    @RECUPERACAO,
                    @QUANTIDADE_QUESTOES
                );SELECT SCOPE_IDENTITY();";

        private const string sqlEditar =
            @"UPDATE [TBTESTE]	
		        SET
			        TITULO = @TITULO,
                    DATA = @DATA,
                    DISCIPLINA_NUMERO = @DISCIPLINA_NUMERO,
                    MATERIA_NUMERO = @MATERIA_NUMERO,
                    RECUPERACAO = @RECUPERACAO,
                    QUANTIDADE_QUESTOES = @QUANTIDADE_QUESTOES
		        WHERE
			        NUMERO = @NUMERO";

        private const string sqlExcluir =
            @"DELETE FROM [TBTESTE]
		        WHERE
			        [NUMERO] = @NUMERO";

        private const string sqlSelecionarTodos =
            @"SELECT
	            T.NUMERO,
	            T.TITULO,
                T.DATA,
	            D.NOME AS DISCIPLINA_NOME,
				D.NUMERO AS DISCIPLINA_NUMERO,
                M.NOME AS MATERIA_NOME,
				M.NUMERO AS MATERIA_NUMERO,
                T.RECUPERACAO,
                T.QUANTIDADE_QUESTOES
            FROM 
	            TBTESTE AS T
            INNER JOIN TBDISCIPLINA AS D ON T.DISCIPLINA_NUMERO = D.NUMERO
	        INNER JOIN TBMATERIA AS M ON T.MATERIA_NUMERO = M.NUMERO";

        private const string sqlSelecionarPorNumero =
            @"SELECT
	            T.NUMERO,
	            T.TITULO,
                T.DATA,
	            D.NOME AS DISCIPLINA_NOME,
				D.NUMERO AS DISCIPLINA_NUMERO,
                M.NOME AS MATERIA_NOME,
				M.NUMERO AS MATERIA_NUMERO,
                T.RECUPERACAO,
                T.QUANTIDADE_QUESTOES
            FROM 
	            TBTESTE AS T
            INNER JOIN TBDISCIPLINA AS D ON T.DISCIPLINA_NUMERO = D.NUMERO
	        INNER JOIN TBMATERIA AS M ON T.MATERIA_NUMERO = M.NUMERO
            WHERE
                    T.NUMERO = @NUMERO";

        private const string sqlSelecionarQuestoesDoTeste =
            @"SELECT
	            *
            FROM 
	            TBQUESTAO AS Q
	            INNER JOIN TBQUESTAO_TBTESTE AS QT 
	            ON Q.NUMERO = QT.QUESTAO_NUMERO
            WHERE
	            QT.TESTE_NUMERO = @TESTE_NUMERO";

        private const string sqlAdicionarQuestaoNoTeste =
            @"INSERT INTO [TBQUESTAO_TBTESTE]
            (
                QUESTAO_NUMERO,
                TESTE_NUMERO
            )
            VALUES
            (
              @QUESTAO_NUMERO,
              @TESTE_NUMERO  
            )";

        private const string sqlRemoverQuestaoDoTeste =
            @"DELETE FROM 
                TBQUESTAO_TBTESTE
             WHERE
                QUESTAO_NUMERO = @QUESTAO_NUMERO";

        #endregion

        public ValidationResult Inserir(Teste teste)
        {
            var validador = new ValidadorTeste();

            var resultadoValidacao = validador.Validate(teste);

            if (resultadoValidacao.IsValid == false)
                return resultadoValidacao;

            SqlConnection conexaoComBanco = new SqlConnection(enderecoBanco);

            SqlCommand comandoInsercao = new SqlCommand(sqlInserir, conexaoComBanco);

            ConfigurarParametrosTeste(teste, comandoInsercao);

            conexaoComBanco.Open();
            var id = comandoInsercao.ExecuteScalar();
            teste.Numero = Convert.ToInt32(id);

            conexaoComBanco.Close();

            return resultadoValidacao;
        }

        public ValidationResult Inserir(Teste teste, List<QuestaoObjetiva> questoesSorteadas)
        {
            var resultadoValidacao = Inserir(teste);

            if (resultadoValidacao.IsValid == false)
                return resultadoValidacao;

            foreach (var questao in questoesSorteadas)
            {
                AdicionarQuestao(teste, questao);
            }

            return resultadoValidacao;
        }

        public ValidationResult Editar(Teste teste)
        {
            var validador = new ValidadorTeste();

            var resultadoValidacao = validador.Validate(teste);

            if (resultadoValidacao.IsValid == false)
                return resultadoValidacao;

            SqlConnection conexaoComBanco = new SqlConnection(enderecoBanco);

            SqlCommand comandoEdicao = new SqlCommand(sqlEditar, conexaoComBanco);

            comandoEdicao.Parameters.AddWithValue("NUMERO", teste.Numero);

            ConfigurarParametrosTeste(teste, comandoEdicao);

            conexaoComBanco.Open();
            comandoEdicao.ExecuteNonQuery();
            conexaoComBanco.Close();

            return resultadoValidacao;
        }

        public ValidationResult Editar(Teste teste, List<QuestaoObjetiva> questoesSorteadas)
        {
            var resultadoValidacao = Editar(teste);

            if (resultadoValidacao.IsValid == false)
                return resultadoValidacao;

            foreach (var questao in questoesSorteadas)
            {
                if (teste.QuestoesObjetivas.Contains(questao))
                    RemoverQuestao(questao);
            }

            foreach (var questao in questoesSorteadas)
            {
                AdicionarQuestao(teste, questao);
            }

            return resultadoValidacao;
        }

        public ValidationResult Excluir(Teste teste)
        {
            foreach (var questao in teste.QuestoesObjetivas)
            {
                RemoverQuestao(questao);
            }

            SqlConnection conexaoComBanco = new SqlConnection(enderecoBanco);

            SqlCommand comandoExclusao = new SqlCommand(sqlExcluir, conexaoComBanco);

            comandoExclusao.Parameters.AddWithValue("NUMERO", teste.Numero);

            conexaoComBanco.Open();
            int numeroRegistrosExcluidos = comandoExclusao.ExecuteNonQuery();

            var resultadoValidacao = new ValidationResult();

            if (numeroRegistrosExcluidos == 0)
                resultadoValidacao.Errors.Add(new ValidationFailure("", "Não foi possível remover o registro"));

            conexaoComBanco.Close();

            return resultadoValidacao;
        }

        public Teste SelecionarPorNumero(int numero)
        {
            SqlConnection conexaoComBanco = new SqlConnection(enderecoBanco);

            SqlCommand comandoSelecao = new SqlCommand(sqlSelecionarPorNumero, conexaoComBanco);

            comandoSelecao.Parameters.AddWithValue("NUMERO", numero);

            conexaoComBanco.Open();
            SqlDataReader leitorTeste = comandoSelecao.ExecuteReader();

            Teste teste = null;
            if (leitorTeste.Read())
                teste = ConverterParaTeste(leitorTeste);

            conexaoComBanco.Close();

            CarregarQuestoes(teste);

            return teste;
        }

        public List<Teste> SelecionarTodos()
        {
            SqlConnection conexaoComBanco = new SqlConnection(enderecoBanco);

            SqlCommand comandoSelecao = new SqlCommand(sqlSelecionarTodos, conexaoComBanco);

            conexaoComBanco.Open();
            SqlDataReader leitorTeste = comandoSelecao.ExecuteReader();

            List<Teste> testes = new List<Teste>();

            while (leitorTeste.Read())
            {
                var teste = ConverterParaTeste(leitorTeste);

                testes.Add(teste);
            }

            conexaoComBanco.Close();

            return testes;
        }

        private static Teste ConverterParaTeste(SqlDataReader leitorTeste)
        {
            var numero = Convert.ToInt32(leitorTeste["NUMERO"]);
            var titulo = Convert.ToString(leitorTeste["TITULO"]);
            var data = Convert.ToDateTime(leitorTeste["DATA"]);
            var recuperacao = Convert.ToBoolean(leitorTeste["RECUPERACAO"]);
            var qtdQuestoes = Convert.ToInt32(leitorTeste["QUANTIDADE_QUESTOES"]);

            var numeroDisciplina = Convert.ToInt32(leitorTeste["DISCIPLINA_NUMERO"]);
            var nomeDisciplina = Convert.ToString(leitorTeste["DISCIPLINA_NOME"]);

            var numeroMateria = Convert.ToInt32(leitorTeste["MATERIA_NUMERO"]);
            var nomeMateria = Convert.ToString(leitorTeste["MATERIA_NOME"]);


            var teste = new Teste
            {
                Numero = numero,
                Titulo = titulo,
                Data = data,
                Recuperacao = recuperacao,
                QuantidadeQuestoes = qtdQuestoes,
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

            return teste;
        }

        private void ConfigurarParametrosTeste(Teste teste, SqlCommand comando)
        {
            comando.Parameters.AddWithValue("TITULO", teste.Titulo);
            comando.Parameters.AddWithValue("DATA", teste.Data);
            comando.Parameters.AddWithValue("RECUPERACAO", teste.Recuperacao);
            comando.Parameters.AddWithValue("QUANTIDADE_QUESTOES", teste.QuantidadeQuestoes);
            comando.Parameters.AddWithValue("DISCIPLINA_NUMERO", teste.Disciplina.Numero);
            comando.Parameters.AddWithValue("MATERIA_NUMERO", teste.Materia.Numero);
        }

        private void CarregarQuestoes(Teste teste)
        {
            SqlConnection conexaoComBanco = new SqlConnection(enderecoBanco);

            SqlCommand comandoSelecao = new SqlCommand(sqlSelecionarQuestoesDoTeste, conexaoComBanco);

            comandoSelecao.Parameters.AddWithValue("TESTE_NUMERO", teste.Numero);

            conexaoComBanco.Open();
            SqlDataReader leitorQuestao = comandoSelecao.ExecuteReader();

            while (leitorQuestao.Read())
            {
                var questao = ConverterParaQuestao(leitorQuestao);

                teste.QuestoesObjetivas.Add(questao);
            }

            conexaoComBanco.Close();
        }

        private QuestaoObjetiva ConverterParaQuestao(SqlDataReader leitorQuestao)
        {
            var numero = Convert.ToInt32(leitorQuestao["NUMERO"]);
            var enunciado = Convert.ToString(leitorQuestao["ENUNCIADO"]);

            var questao = new QuestaoObjetiva
            {
                Numero = numero,
                Enunciado = enunciado
            };

            return questao;
        }

        private void AdicionarQuestao(Teste teste, QuestaoObjetiva questaoSorteada)
        {
            SqlConnection conexaoComBanco = new SqlConnection(enderecoBanco);

            SqlCommand comandoInsercao = new SqlCommand(sqlAdicionarQuestaoNoTeste, conexaoComBanco);

            comandoInsercao.Parameters.AddWithValue("QUESTAO_NUMERO", questaoSorteada.Numero);
            comandoInsercao.Parameters.AddWithValue("TESTE_NUMERO", teste.Numero);

            conexaoComBanco.Open();
            var id = comandoInsercao.ExecuteNonQuery();

            conexaoComBanco.Close();
        }

        private void RemoverQuestao(QuestaoObjetiva questao)
        {
            SqlConnection conexaoComBanco = new SqlConnection(enderecoBanco);

            SqlCommand comandoExclusao = new SqlCommand(sqlRemoverQuestaoDoTeste, conexaoComBanco);

            comandoExclusao.Parameters.AddWithValue("QUESTAO_NUMERO", questao.Numero);

            conexaoComBanco.Open();
            int numeroRegistrosExcluidos = comandoExclusao.ExecuteNonQuery();

            conexaoComBanco.Close();
        }
    }
}
