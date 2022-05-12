using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Testes.Dominio.ModuloDisciplina;
using Testes.Dominio.ModuloMateria;
using Testes.Dominio.ModuloQuestao;

namespace Testes.WinApp.ModuloQuestao
{
    public partial class TelaCadastroQuestoesForm : Form
    {
        private QuestaoObjetiva _questaoObjetiva; 
        private ControladorQuestao _controladorQuestao;
        private List<Materia> _materias;

        public TelaCadastroQuestoesForm(List<Disciplina> disciplinas, List<Materia> materias)
        {
            InitializeComponent();

            _materias = materias;

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

        public Func<QuestaoObjetiva, ValidationResult> GravarRegistro { get; set; }

        public QuestaoObjetiva QuestaoObjetiva
        {
            get { return _questaoObjetiva; }
            set
            {
                _questaoObjetiva = value;

                txtNumero.Text = _questaoObjetiva.Numero.ToString();
                txtEnunciado.Text = _questaoObjetiva.Enunciado;
                txtGabarito.Text = _questaoObjetiva.Gabarito;
                cmbDisciplinas.SelectedItem = _questaoObjetiva.Disciplina;
                cmbMaterias.SelectedItem = _questaoObjetiva.Materia;
            }
        }

        private void btnGravar_Click(object sender, EventArgs e)
        {
            _questaoObjetiva.Enunciado = txtEnunciado.Text;
            _questaoObjetiva.Gabarito = txtGabarito.Text;
            _questaoObjetiva.Disciplina = (Disciplina)cmbDisciplinas.SelectedItem;
            _questaoObjetiva.Materia = (Materia)cmbMaterias.SelectedItem;

            var resultadoValidacao = GravarRegistro(QuestaoObjetiva);

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
        }
    }
}
