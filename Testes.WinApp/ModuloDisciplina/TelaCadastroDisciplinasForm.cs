using FluentValidation.Results;
using System;
using System.Windows.Forms;
using Testes.Dominio.ModuloDisciplina;

namespace Testes.WinApp.ModuloDisciplina
{
    public partial class TelaCadastroDisciplinasForm : Form
    {
        private Disciplina disciplina;
        private ControladorDisciplina ControladorDisciplina;


        public TelaCadastroDisciplinasForm()
        {
            InitializeComponent();
        }

        
        public Func<Disciplina, ValidationResult> GravarRegistro { get; set; }

        public Disciplina Disciplina
        {
            get { return disciplina; }
            set
            {
                disciplina = value;

                txtNumero.Text = disciplina.Numero.ToString();
                txtNome.Text = disciplina.Nome;
            }
        }

        private void btnGravar_Click(object sender, EventArgs e)
        {
            disciplina.Nome = txtNome.Text;

            var resultadoValidacao = GravarRegistro(disciplina);

            if (resultadoValidacao.IsValid == false)
            {
                string erro = resultadoValidacao.Errors[0].ErrorMessage;

                TelaPrincipalForm.Instancia.AtualizarRodape(erro);

                DialogResult = DialogResult.None;
            }
        }

        private void TelaCadastroDisciplinasForm_Load(object sender, EventArgs e)
        {
            TelaPrincipalForm.Instancia.AtualizarRodape("");
        }

        private void TelaCadastroDisciplinasForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            TelaPrincipalForm.Instancia.AtualizarRodape("");
        }
    }
}
