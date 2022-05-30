using Testes.Dominio.Compartilhado;

namespace Testes.Dominio.ModuloQuestao
{
    public class Alternativa : EntidadeBase<Alternativa>
    {
        public int Indice { get; set; }

        public string Resposta { get; set; }

        public bool Correta { get; set; }

        public int Questao_Numero { get; set; }

        public override void Atualizar(Alternativa registro)
        {
        }
    }
}