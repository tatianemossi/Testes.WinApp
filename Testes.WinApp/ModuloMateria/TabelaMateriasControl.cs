using System.Collections.Generic;
using System.Windows.Forms;
using Testes.Dominio.ModuloMateria;
using Testes.WinApp.Compartilhado;

namespace Testes.WinApp.ModuloMateria
{
    public partial class TabelaMateriasControl : UserControl
    {
        public TabelaMateriasControl()
        {
            InitializeComponent();
            grid.ConfigurarGridZebrado();
            grid.ConfigurarGridSomenteLeitura();
            grid.Columns.AddRange(ObterColunas());
        }

        public DataGridViewColumn[] ObterColunas()
        {
            var colunas = new DataGridViewColumn[]
            {
                new DataGridViewTextBoxColumn { DataPropertyName = "Numero", HeaderText = "Numero"},

                new DataGridViewTextBoxColumn { DataPropertyName = "Nome", HeaderText = "Nome"},

                new DataGridViewTextBoxColumn { DataPropertyName = "Série", HeaderText = "Série"},

                new DataGridViewTextBoxColumn { DataPropertyName = "Disciplina_Numero", HeaderText = "Número Disciplina" },

                new DataGridViewTextBoxColumn { DataPropertyName = "Disciplina_Nome", HeaderText = "Disciplina" },
            };

            return colunas;
        }

        public int ObtemNumeroMateriaSelecionada()
        {
            return grid.SelecionarNumero<int>();
        }

        public void AtualizarRegistros(List<Materia> materias)
        {
            grid.Rows.Clear();

            foreach (var materia in materias)
            {
                grid.Rows.Add(materia.Numero, materia.Nome, materia.Serie, materia.Disciplina?.Nome, materia.NumeroDisciplina);
            }
        }
    }
}
