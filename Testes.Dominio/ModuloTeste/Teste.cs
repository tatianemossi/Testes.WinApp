using System;
using System.Collections.Generic;
using System.Linq;
using Testes.Dominio.Compartilhado;
using Testes.Dominio.ModuloDisciplina;
using Testes.Dominio.ModuloMateria;
using Testes.Dominio.ModuloQuestao;

namespace Testes.Dominio.ModuloTeste
{
    public class Teste : EntidadeBase<Teste>
    {
        public string Titulo { get; set; }

        public DateTime Data { get; set; }

        public Disciplina Disciplina { get; set; }

        public Materia Materia { get; set; }

        public bool Recuperacao { get; set; }

        public int QuantidadeQuestoes { get; set; }

        public List<QuestaoObjetiva> QuestoesObjetivas { get; set; }

        public int NumeroDisciplina { get; set; }
        
        public int NumeroMateria { get; set; }

        public override void Atualizar(Teste registro)
        {
        }

        public Teste()
        {
            QuestoesObjetivas = new();
        }

        public Teste Clone()
        {
            var questoesCopiadas = new QuestaoObjetiva[this.QuestoesObjetivas.Count];

            this.QuestoesObjetivas.CopyTo(questoesCopiadas);

            return new Teste
            {
                Titulo = this.Titulo,
                Data = this.Data,
                Disciplina = this.Disciplina,
                Materia = this.Materia,
                Recuperacao = this.Recuperacao,
                QuantidadeQuestoes = this.QuantidadeQuestoes,
                QuestoesObjetivas = questoesCopiadas.ToList()
            };
        }
    }
}
