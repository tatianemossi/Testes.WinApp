﻿using System.Collections.Generic;
using Testes.Dominio.Compartilhado;
using Testes.Dominio.ModuloDisciplina;
using Testes.Dominio.ModuloMateria;

namespace Testes.Dominio.ModuloQuestao
{
    public class QuestaoObjetiva : EntidadeBase<QuestaoObjetiva>
    {
        public QuestaoObjetiva()
        {
            Alternativas = new List<Alternativa>();
        }

        public string Enunciado { get; set; }

        public string Bimestre { get; set; }


        public int NumeroMateria { get; set; }

        public Materia Materia { get; set; }


        public int NumeroDisciplina { get; set; }

        public Disciplina Disciplina { get; set; }


        public List<Alternativa> Alternativas { get; set; }

        public override void Atualizar(QuestaoObjetiva registro)
        {
        }
    }
}
