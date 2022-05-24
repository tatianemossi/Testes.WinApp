using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Testes.Dominio.ModuloDisciplina;
using Testes.Dominio.ModuloMateria;

namespace testes.Infra.BancoDados.ModuloMateria
{
    public class RepositorioMateriaEmBancoDados : IRepositorioMateria
    {
        private const string enderecoBanco =
               "Data Source=(LocalDB)\\MSSqlLocalDB;" +
               "Initial Catalog=testesDb;" +
               "Integrated Security=True;" +
               "Pooling=False";

        #region Sql Queries
        private const string sqlInserir =
            @"INSERT INTO [TBMATERIA] 
                (
                    [NOME],
                    [SERIE],
                    [DISCIPLINA_NUMERO]
	            )
	            VALUES
                (
                    @NOME,
                    @SERIE,
                    @DISCIPLINA_NUMERO

                );SELECT SCOPE_IDENTITY();";

        private const string sqlEditar =
            @"UPDATE [TBMATERIA]	
		        SET
			        NOME = @NOME,
                    SERIE = @SERIE,
                    DISCIPLINA_NUMERO = @DISCIPLINA_NUMERO
		        WHERE
			        NUMERO = @NUMERO";

        private const string sqlExcluir =
            @"DELETE FROM [TBMATERIA]
		        WHERE
			        [NUMERO] = @NUMERO";

        private const string sqlSelecionarTodos =
            @"SELECT
	            MT.NUMERO,
	            MT.NOME,
	            MT.SERIE,
	            MT.DISCIPLINA_NUMERO,
	            D.NOME AS DISCIPLINA_NOME
            FROM 
	            TBMATERIA AS MT INNER JOIN
	            TBDISCIPLINA AS D ON 
	            MT.DISCIPLINA_NUMERO = D.NUMERO";

        private const string sqlSelecionarPorNumero =
            @"SELECT 
		            MT.NUMERO, 
		            MT.NOME,
                    MT.SERIE,
                    MT.DISCIPLINA_NUMERO,
                    D.NOME AS DISCIPLINA_NOME
	         FROM 
		            TBMATERIA AS MT INNER JOIN
	                TBDISCIPLINA AS D ON 
	                MT.DISCIPLINA_NUMERO = D.NUMERO
             WHERE
                    MT.NUMERO = @NUMERO";

        private const string sqlSelecionarPorNome =
            @"SELECT 
		            [NOME]
	            FROM 
		            [TBMATERIA]
		        WHERE
                    [NOME] = @NOME";

        private const string sqlSelecionarMateriaPeloNumeroDisciplina =
            @"SELECT 
	            *
            FROM 
	            [DBO].[TBMATERIA]
            WHERE
	            DISCIPLINA_NUMERO = NUMERO";

        #endregion

        public ValidationResult Inserir(Materia materia)
        {
            SqlConnection conexaoComBanco = new SqlConnection(enderecoBanco);

            SqlCommand comandoInsercao = new SqlCommand(sqlInserir, conexaoComBanco);

            ConfigurarParametrosMateria(materia, comandoInsercao);

            conexaoComBanco.Open();
            var id = comandoInsercao.ExecuteScalar();
            materia.Numero = Convert.ToInt32(id);

            conexaoComBanco.Close();

            return new ValidationResult();
        }

        public ValidationResult Editar(Materia materia)
        {
            var validador = new ValidadorMateria();

            var resultadoValidacao = validador.Validate(materia);

            if (resultadoValidacao.IsValid == false)
                return resultadoValidacao;

            SqlConnection conexaoComBanco = new SqlConnection(enderecoBanco);

            SqlCommand comandoEdicao = new SqlCommand(sqlEditar, conexaoComBanco);

            comandoEdicao.Parameters.AddWithValue("NUMERO", materia.Numero);

            ConfigurarParametrosMateria(materia, comandoEdicao);

            conexaoComBanco.Open();
            comandoEdicao.ExecuteNonQuery();
            conexaoComBanco.Close();

            return resultadoValidacao;
        }

        public ValidationResult Excluir(Materia materia)
        {
            SqlConnection conexaoComBanco = new SqlConnection(enderecoBanco);

            SqlCommand comandoExclusao = new SqlCommand(sqlExcluir, conexaoComBanco);

            comandoExclusao.Parameters.AddWithValue("NUMERO", materia.Numero);

            conexaoComBanco.Open();
            int numeroRegistrosExcluidos = comandoExclusao.ExecuteNonQuery();

            var resultadoValidacao = new ValidationResult();

            if (numeroRegistrosExcluidos == 0)
                resultadoValidacao.Errors.Add(new ValidationFailure("", "Não foi possível remover o registro"));

            conexaoComBanco.Close();

            return resultadoValidacao;
        }

        public List<Materia> SelecionarTodos()
        {
            SqlConnection conexaoComBanco = new SqlConnection(enderecoBanco);

            SqlCommand comandoSelecao = new SqlCommand(sqlSelecionarTodos, conexaoComBanco);

            conexaoComBanco.Open();
            SqlDataReader leitorMateria = comandoSelecao.ExecuteReader();

            List<Materia> materias = new List<Materia>();

            while (leitorMateria.Read())
            {
                var materia = ConverterParaMateria(leitorMateria);

                materias.Add(materia);
            }

            conexaoComBanco.Close();

            return materias;
        }

        public Materia SelecionarPorNumero(int numero)
        {
            SqlConnection conexaoComBanco = new SqlConnection(enderecoBanco);

            SqlCommand comandoSelecao = new SqlCommand(sqlSelecionarPorNumero, conexaoComBanco);

            comandoSelecao.Parameters.AddWithValue("NUMERO", numero);

            conexaoComBanco.Open();
            SqlDataReader leitorMateria = comandoSelecao.ExecuteReader();

            Materia materia = null;
            if (leitorMateria.Read())
                materia = ConverterParaMateria(leitorMateria);

            conexaoComBanco.Close();

            return materia;
        }

        private static Materia ConverterParaMateria(SqlDataReader leitorMateria)
        {
            var numero = Convert.ToInt32(leitorMateria["NUMERO"]);
            var nome = Convert.ToString(leitorMateria["NOME"]);
            var serie = Convert.ToString(leitorMateria["SERIE"]);

            var numeroDisciplina = Convert.ToInt32(leitorMateria["DISCIPLINA_NUMERO"]);
            var nomeDisciplina = Convert.ToString(leitorMateria["DISCIPLINA_NOME"]);


            var materia = new Materia
            {
                Numero = numero,
                Nome = nome,
                Serie = serie,
                NumeroDisciplina = numeroDisciplina,
                Disciplina = new Disciplina
                {
                    Numero = numeroDisciplina,
                    Nome = nomeDisciplina
                }

            };

            return materia;
        }

        private static void ConfigurarParametrosMateria(Materia novaMateria, SqlCommand comando)
        {
            comando.Parameters.AddWithValue("NOME", novaMateria.Nome);
            comando.Parameters.AddWithValue("SERIE", novaMateria.Serie);
            comando.Parameters.AddWithValue("DISCIPLINA_NUMERO", novaMateria.NumeroDisciplina);
            comando.Parameters.AddWithValue("DISCIPLINA_NOME", novaMateria.Disciplina.Nome);
        }

        public bool MateriaJaExiste(string nome, int numero)
        {
            SqlConnection conexaoComBanco = new SqlConnection(enderecoBanco);

            try
            {
                SqlCommand comandoSelecao = new SqlCommand(sqlSelecionarPorNome, conexaoComBanco);

                comandoSelecao.Parameters.AddWithValue("NOME", nome);

                conexaoComBanco.Open();

                SqlDataReader leitorMateria = comandoSelecao.ExecuteReader();

                if (leitorMateria.Read())
                    return true;
                else
                    return false;
            }
            catch
            {
                throw;
            }
            finally
            {
                conexaoComBanco.Close();
            }
        }

        public bool ExisteMateriaPeloNumeroDisciplina(int numero)
        {
            SqlConnection conexaoComBanco = new SqlConnection(enderecoBanco);

            try
            {
                SqlCommand comandoSelecao = new SqlCommand(sqlSelecionarPorNome, conexaoComBanco);

                comandoSelecao.Parameters.AddWithValue("NUMERO", numero);

                conexaoComBanco.Open();

                SqlDataReader leitorMateria = comandoSelecao.ExecuteReader();

                if (leitorMateria.Read())
                    return true;
                else
                    return false;
            }
            catch
            {
                throw;
            }
            finally
            {
                conexaoComBanco.Close();
            }
        }
    }
}
