using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Testes.Dominio.ModuloQuestao;

namespace testes.Infra.BancoDados.ModuloQuestao
{
    public class RepositorioAlternativaEmBancoDados : IRepositorioAlternativa
    {
        private const string enderecoBanco =
               "Data Source=(LocalDB)\\MSSqlLocalDB;" +
               "Initial Catalog=testesDb;" +
               "Integrated Security=True;" +
               "Pooling=False";

        #region Sql Queries

        private const string sqlExcluir = @"
            DELETE FROM TBALTERNATIVA 
            WHERE QUESTAO_NUMERO = @QUESTAO_NUMERO";

        private const string sqlInserir = @"
            INSERT INTO TBALTERNATIVA 
                (INDICE, RESPOSTA, CORRETA, QUESTAO_NUMERO) 
            VALUES 
                (@INDICE, @RESPOSTA, @CORRETA, @QUESTAO_NUMERO)";

        private const string sqlSelecionarPorNumeroQuestao = @"
            SELECT 
                * 
            FROM 
                TBALTERNATIVA 
            WHERE 
                QUESTAO_NUMERO = @QUESTAO_NUMERO";

        #endregion

        public void Inserir(Alternativa alternativa)
        {
            SqlConnection conexaoComBanco = new SqlConnection(enderecoBanco);

            SqlCommand comandoInsercao = new SqlCommand(sqlInserir, conexaoComBanco);

            comandoInsercao.Parameters.AddWithValue("RESPOSTA", alternativa.Resposta);
            comandoInsercao.Parameters.AddWithValue("INDICE", alternativa.Indice);
            comandoInsercao.Parameters.AddWithValue("CORRETA", alternativa.Correta);
            comandoInsercao.Parameters.AddWithValue("QUESTAO_NUMERO", alternativa.Questao_Numero);

            conexaoComBanco.Open();
            var id = comandoInsercao.ExecuteScalar();
            alternativa.Numero = Convert.ToInt32(id);

            conexaoComBanco.Close();
        }

        public void DeletarAlternativasPelaNumeroQuestao(int numeroQuestao)
        {
            SqlConnection conexaoComBanco = new SqlConnection(enderecoBanco);

            SqlCommand comandoExclusao = new SqlCommand(sqlExcluir, conexaoComBanco);

            comandoExclusao.Parameters.AddWithValue("QUESTAO_NUMERO", numeroQuestao);

            conexaoComBanco.Open();
            comandoExclusao.ExecuteNonQuery();

            conexaoComBanco.Close();
        }

        public List<Alternativa> SelecionarPorNumeroQuestao(int numeroQuestao)
        {
            SqlConnection conexaoComBanco = new SqlConnection(enderecoBanco);

            SqlCommand comandoSelecao = new SqlCommand(sqlSelecionarPorNumeroQuestao, conexaoComBanco);

            comandoSelecao.Parameters.AddWithValue("QUESTAO_NUMERO", numeroQuestao);

            conexaoComBanco.Open();
            SqlDataReader leitorAlternativa = comandoSelecao.ExecuteReader();

            List<Alternativa> alternativas = new List<Alternativa>();

            while (leitorAlternativa.Read())
            {
                var alternativa = new Alternativa
                {
                    Numero = Convert.ToInt32(leitorAlternativa["NUMERO"]),
                    Indice = Convert.ToInt32(leitorAlternativa["INDICE"]),
                    Resposta = Convert.ToString(leitorAlternativa["RESPOSTA"]),
                    Correta = Convert.ToBoolean(leitorAlternativa["CORRETA"]),
                    Questao_Numero = numeroQuestao
                };

                alternativas.Add(alternativa);
            }

            conexaoComBanco.Close();

            return alternativas;
        }
    }
}
