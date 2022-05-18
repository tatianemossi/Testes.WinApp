using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Testes.Dominio.ModuloDisciplina;
using Testes.Dominio.ModuloMateria;

namespace Testes.WinApp.ModuloMateria
{
    public partial class TelaCadastroMateriasForm : Form
    {
        private Materia _materia;
        private ControladorMateria _controladorMateria;
        private IRepositorioMateria _repositorioMateria;
        private List<Disciplina> _disciplinas;

        public TelaCadastroMateriasForm(List<Disciplina> disciplinas, IRepositorioMateria repositorioMateria)
        {
            InitializeComponent();

            _repositorioMateria = repositorioMateria;

            _disciplinas = disciplinas;

            CarregarDisciplinas();
        }

        private void CarregarDisciplinas()
        {
            cmbDisciplinas.Items.Clear();

            foreach (var item in _disciplinas)
            {
                cmbDisciplinas.Items.Add(item);
            }
        }

        public Func<Materia, ValidationResult> GravarRegistro { get; set; }

        public Materia Materia
        {
            get { return _materia; }
            set
            {
                _materia = value;

                txtNumero.Text = Materia.Numero.ToString();
                txtNome.Text = Materia.Nome;
                cmbDisciplinas.SelectedItem = _materia.Disciplina;

                DefinirSerieSelecionada();

                DefinirDisciplinaSelecionada();
            }
        }

        private void btnGravar_Click(object sender, EventArgs e)
        {
            _materia.Nome = txtNome.Text;
            _materia.Disciplina = (Disciplina)cmbDisciplinas.SelectedItem;
            _materia.Serie = VerificarSerieMarcada();

            if (_repositorioMateria.MateriaJaExiste(_materia.Nome, _materia.Numero))
            {
                MessageBox.Show("Já existe uma matéria com este nome.",
                    "Inserindo Matéria",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
                DialogResult = DialogResult.None;
            }
            else
            {
                var resultadoValidacao = GravarRegistro(Materia);

                if (resultadoValidacao.IsValid == false)
                {
                    string erro = resultadoValidacao.Errors[0].ErrorMessage;

                    TelaPrincipalForm.Instancia.AtualizarRodape(erro);

                    DialogResult = DialogResult.None;
                }
            }
        }

        public string VerificarSerieMarcada()
        {
            foreach (RadioButton radioButton in grupoSerie.Controls)
            {
                if (radioButton.Checked)
                {
                    return radioButton.Text; 
                }
            }
            return string.Empty;
        }

        private void TelaCadastroMateriasForm_Load(object sender, EventArgs e)
        {
            TelaPrincipalForm.Instancia.AtualizarRodape("");
        }

        private void TelaCadastroMateriasForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            TelaPrincipalForm.Instancia.AtualizarRodape("");
        }

        private void DefinirSerieSelecionada()
        {
            rbPrimeiraSerie.Checked = _materia.Serie == "1ª Série";
            rbSegundaSerie.Checked = _materia.Serie == "2ª Série";
        }

        private void DefinirDisciplinaSelecionada()
        {
            var disciplina = _disciplinas.FirstOrDefault(x => x.Numero == _materia.Disciplina?.Numero);
            cmbDisciplinas.SelectedItem = disciplina;
        }
    }
}
