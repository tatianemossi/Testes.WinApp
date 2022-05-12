using System;
using System.Collections.Generic;
using Testes.Dominio.Compartilhado;
using Testes.Dominio.ModuloDisciplina;
using Testes.Dominio.ModuloQuestao;

namespace Testes.Dominio.ModuloTeste
{
    public class Teste : EntidadeBase<Teste>
    {
        public int NumeroQuestoes { get; set; }

        public Disciplina Disciplina { get; set; }

        public DateTime Data { get; set; }

        public List<QuestaoObjetiva> QuestoesObjetivas { get; set;}

        public override void Atualizar(Teste registro)
        {
        }
    }
}
