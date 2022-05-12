using System;
using System.Collections.Generic;
using Testes.Dominio.Compartilhado;
using Testes.Dominio.ModuloDisciplina;
using Testes.Dominio.ModuloMateria;
using Testes.Dominio.ModuloQuestao;

namespace Testes.Dominio.ModuloTeste
{
    public class Teste : EntidadeBase<Teste>
    {
        public int NumeroQuestoes { get; set; }

        public string Titulo { get; set; }

        public DateTime Data { get; set; }

        public Disciplina Disciplina { get; set; }

        public Materia Materia { get; set; }

        public bool Recuperacao { get; set; }

        public List<QuestaoObjetiva> QuestoesObjetivas { get; set;}

        public override void Atualizar(Teste registro)
        {
        }
    }
}
