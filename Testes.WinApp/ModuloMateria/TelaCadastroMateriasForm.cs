using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Testes.Dominio.ModuloDisciplina;
using Testes.Dominio.ModuloMateria;

namespace Testes.WinApp.ModuloMateria
{
    public partial class TelaCadastroMateriasForm : Form
    {
        private Materia materia;
        private ControladorMateria ControladorMateria;


        public TelaCadastroMateriasForm(List<Disciplina> disciplinas)
        {
            InitializeComponent();

            CarregarDisciplinas(disciplinas);
        }

        private void CarregarDisciplinas(List<Disciplina> disciplinas)
        {
            cmbDisciplinas.Items.Clear();

            foreach (var item in disciplinas)
            {
                cmbDisciplinas.Items.Add(item);
            }
        }

        public Func<Materia, ValidationResult> GravarRegistro { get; set; }

        public Materia Materia
        {
            get { return materia; }
            set
            {
                materia = value;

                txtNumero.Text = Materia.Numero.ToString();
                txtNome.Text = Materia.Nome;
                cmbDisciplinas.SelectedItem = materia.Disciplina;
            }
        }

        private void btnGravar_Click(object sender, EventArgs e)
        {
            materia.Nome = txtNome.Text;
            materia.Disciplina = (Disciplina)cmbDisciplinas.SelectedItem;
            materia.Serie = VerificarSerieMarcada();

            var resultadoValidacao = GravarRegistro(Materia);

            if (resultadoValidacao.IsValid == false)
            {
                string erro = resultadoValidacao.Errors[0].ErrorMessage;

                TelaPrincipalForm.Instancia.AtualizarRodape(erro);

                DialogResult = DialogResult.None;
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
    }
}
