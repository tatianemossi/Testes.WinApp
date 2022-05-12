using Testes.Dominio.Compartilhado;
using Testes.Dominio.ModuloDisciplina;
using Testes.Dominio.ModuloMateria;

namespace Testes.Dominio.ModuloQuestao
{
    public class QuestaoObjetiva : EntidadeBase<QuestaoObjetiva>
    {
        public string Bimestre { get; set; }

        public Materia Materia { get; set; }

        public Disciplina Disciplina { get; set; }

        public string Enunciado { get; set; }

        public string Gabarito { get; set; }

        public override void Atualizar(QuestaoObjetiva registro)
        {
        }
    }
}
