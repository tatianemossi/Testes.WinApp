using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Testes.Dominio.ModuloDisciplina;
using Testes.Dominio.ModuloMateria;
using Testes.Dominio.ModuloQuestao;
using Testes.Dominio.ModuloTeste;

namespace Testes.WinApp.ModutoTeste
{
    public partial class TelaCadastroTestesForm : Form
    {
        private Teste _teste;
        private List<Materia> _materias;
        private List<Disciplina> _disciplinas;
        private List<QuestaoObjetiva> _questoes;

        public TelaCadastroTestesForm(List<Disciplina> disciplinas, List<Materia> materias, List<QuestaoObjetiva> questoes)
        {
            InitializeComponent();

            _materias = materias;
            _disciplinas = disciplinas;
            _questoes = questoes;

            CarregarDisciplinas(disciplinas);
            CarregarMaterias(materias);
        }

        private void CarregarMaterias(List<Materia> materias)
        {
            cmbMaterias.Items.Clear();

            foreach (var item in materias)
            {
                cmbMaterias.Items.Add(item);
            }
        }

        private void CarregarDisciplinas(List<Disciplina> disciplinas)
        {
            cmbDisciplinas.Items.Clear();

            foreach (var item in disciplinas)
            {
                cmbDisciplinas.Items.Add(item);
            }
        }

        public Func<Teste, ValidationResult> GravarRegistro { get; set; }

        public Teste Teste
        {
            get { return _teste; }
            set
            {
                _teste = value;

                txtNumero.Text = _teste.Numero.ToString();
                txtTitulo.Text = _teste.Titulo;
                txtQtdQuestoes.Text = _teste.NumeroQuestoes.ToString();
                checkBoxRecuperacao.Checked = _teste.Recuperacao;

                DefinirMateriaSelecionada();

                DefinirDisciplinaSelecionada();

                if (_teste.Data > DateTime.MinValue)
                    dtData.Value = _teste.Data;
            }
        }

        private void DefinirDisciplinaSelecionada()
        {
            var disciplina = _disciplinas.FirstOrDefault(x => x.Numero == _teste.Disciplina?.Numero);
            cmbDisciplinas.SelectedItem = disciplina;
        }

        private void DefinirMateriaSelecionada()
        {
            var materia = _materias.FirstOrDefault(x => x.Numero == _teste.Materia?.Numero);
            cmbMaterias.SelectedItem = materia;
        }

        private void btnGravar_Click(object sender, EventArgs e)
        {
            _teste.Titulo = txtTitulo.Text;
            _teste.Disciplina = (Disciplina)cmbDisciplinas.SelectedItem;
            _teste.Materia = (Materia)cmbMaterias.SelectedItem;
            _teste.NumeroQuestoes = Convert.ToInt32(txtQtdQuestoes.Text);
            _teste.Data = dtData.Value;
            _teste.Recuperacao = checkBoxRecuperacao.Checked;

            var resultadoValidacao = GravarRegistro(Teste);

            if (resultadoValidacao.IsValid == false)
            {
                string erro = resultadoValidacao.Errors[0].ErrorMessage;

                TelaPrincipalForm.Instancia.AtualizarRodape(erro);

                DialogResult = DialogResult.None;
            }
        }


        private void TelaCadastroQuestoesForm_Load(object sender, EventArgs e)
        {
            TelaPrincipalForm.Instancia.AtualizarRodape("");
        }

        private void TelaCadastroQuestoesForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            TelaPrincipalForm.Instancia.AtualizarRodape("");
        }

        private void cmbDisciplinas_SelectedValueChanged(object sender, EventArgs e)
        {
            var disciplina = (Disciplina)cmbDisciplinas.SelectedItem;
            var materiasFiltradas = _materias.Where(x => x.Disciplina.Numero == disciplina.Numero).ToList();

            CarregarMaterias(materiasFiltradas);
            cmbMaterias.Enabled = true;
            DefinirMateriaSelecionada();
        }

        private void txtQtdQuestoes_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
                e.Handled = true;


            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
                e.Handled = true;
        }

        private void btnSortearQuestoes_Click(object sender, EventArgs e)
        {
            var questoesFiltradas = new List<QuestaoObjetiva>();
            var disciplina = (Disciplina)cmbDisciplinas.SelectedItem;

            questoesFiltradas = _questoes.Where(x => x.Disciplina.Numero.Equals(disciplina.Numero)).ToList();

            if (checkBoxRecuperacao.Checked == false)
            {
                var materia = (Materia)cmbMaterias.SelectedItem;
                questoesFiltradas = questoesFiltradas.Where(x => x.Materia.Numero.Equals(materia.Numero)).ToList();
            }

            var qtdQuestoes = Convert.ToInt32(txtQtdQuestoes.Text);

            if (qtdQuestoes > questoesFiltradas.Count)
            {
                MessageBox.Show("O número solicitado ultrapassa o número de questões cadastradas.");
            }
            else
            {
                var questoesJaAdicionadas = new List<int>();
                listQuestoesSorteadas.Items.Clear();
                var random = new Random();
                for (int i = 0; i < qtdQuestoes; i++)
                {
                    var indiceSorteado = random.Next(0, qtdQuestoes);

                    while (questoesJaAdicionadas.Any(x => x == indiceSorteado))
                        indiceSorteado = random.Next(0, qtdQuestoes);

                    var questaoSorteada = questoesFiltradas[indiceSorteado];
                    questoesJaAdicionadas.Add(indiceSorteado);

                    listQuestoesSorteadas.Items.Add(questaoSorteada);                    
                }
            }
        }
    }
}
