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
        private IRepositorioAlternativa _repositorioAlternativa;
        private QuestaoObjetiva _questaoObjetiva;
        private List<Materia> _materias;
        private List<Disciplina> _disciplinas;
        private List<Alternativa> _alternativas = new();

        public TelaCadastroQuestoesForm(List<Disciplina> disciplinas, List<Materia> materias, IRepositorioAlternativa repositorioAlternativa)
        {
            InitializeComponent();

            _materias = materias;
            _disciplinas = disciplinas;
            _repositorioAlternativa = repositorioAlternativa;

            CarregarDisciplinas();
            CarregarMaterias(_materias);
        }

        private void CarregarMaterias(List<Materia> materias)
        {
            cmbMaterias.Items.Clear();

            foreach (var item in materias)
            {
                cmbMaterias.Items.Add(item);
            }
        }

        private void CarregarDisciplinas()
        {
            cmbDisciplinas.Items.Clear();

            foreach (var item in _disciplinas)
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
                _alternativas = _questaoObjetiva.Alternativas;

                DefinirMateriaSelecionada();
                DefinirDisciplinaSelecionada();

                CarregarListaAlternativas();
                CarregarRadioButtonBimestre();
            }
        }

        private void btnGravar_Click(object sender, EventArgs e)
        {
            _questaoObjetiva.Enunciado = txtEnunciado.Text;
            _questaoObjetiva.Disciplina = (Disciplina)cmbDisciplinas.SelectedItem;
            _questaoObjetiva.Materia = (Materia)cmbMaterias.SelectedItem;
            _questaoObjetiva.Bimestre = CarregarBimestre();
            _questaoObjetiva.Alternativas = _alternativas;

            var resultadoValidacao = GravarRegistro(QuestaoObjetiva);

            if (resultadoValidacao.IsValid == true)
            {
                _repositorioAlternativa.DeletarAlternativasPelaNumeroQuestao(_questaoObjetiva.Numero);
                foreach (var alternativa in _alternativas)
                {
                    alternativa.Questao_Numero = _questaoObjetiva.Numero;
                    _repositorioAlternativa.Inserir(alternativa);
                }
            }
            else
            {
                string erro = resultadoValidacao.Errors[0].ErrorMessage;

                TelaPrincipalForm.Instancia.AtualizarRodape(erro);

                DialogResult = DialogResult.None;
            }
        }

        private void CarregarRadioButtonBimestre()
        {
            if (_questaoObjetiva.Bimestre != null)
            {
                if (_questaoObjetiva.Bimestre.Equals("1º Bimestre"))
                    rbPrimeiroBimestre.Checked = true;

                else if (_questaoObjetiva.Bimestre.Equals("2º Bimestre"))
                    rbSegundoBimestre.Checked = true;

                else if (_questaoObjetiva.Bimestre.Equals("3º Bimestre"))
                    rbTerceiroBimestre.Checked = true;

                else if (_questaoObjetiva.Bimestre.Equals("4º Bimestre"))
                    rbQuartoBimestre.Checked = true;
            }
        }

        private string? CarregarBimestre()
        {
            if (rbPrimeiroBimestre.Checked)
                return rbPrimeiroBimestre.Text;

            else if (rbSegundoBimestre.Checked)
                return rbSegundoBimestre.Text;

            else if (rbTerceiroBimestre.Checked)
                return rbTerceiroBimestre.Text;

            else if (rbQuartoBimestre.Checked)
                return rbQuartoBimestre.Text;

            return null;
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

        private void btnAdicionarResposta_Click(object sender, EventArgs e)
        {
            if (AlternativaEhValida())
            {
                if (labelLetraAlternativa.Text.Length > 0)
                    _alternativas.RemoveAll(x => x.Indice == BuscarIndiceAlternativa());

                var novaAlternativa = new Alternativa
                {
                    Indice = BuscarIndiceAlternativa(),
                    Resposta = txtResposta.Text,
                    Correta = checkBoxAlternativaCorreta.Checked,
                    Questao_Numero = _questaoObjetiva.Numero
                };

                _alternativas.Add(novaAlternativa);

                CarregarListaAlternativas();

                checkBoxAlternativaCorreta.Checked = false;
                txtResposta.Text = "";
                labelLetraAlternativa.Text = "";

                DesabilitaBotaoAdicionarResposta();
            }
        }

        private void DesabilitaBotaoAdicionarResposta()
        {
            if (_alternativas.Count == 5)
                btnAdicionarResposta.Enabled = false;
        }

        private int BuscarIndiceAlternativa()
        {
            if (labelLetraAlternativa.Text.Length > 0)
            {
                char charLetra = char.Parse(labelLetraAlternativa.Text.ToUpper());

                return charLetra;
            }

            return _alternativas.Count + 65;
        }

        private void CarregarListaAlternativas()
        {
            listAlternativas.Items.Clear();

            foreach (var alternativa in _alternativas.OrderBy(x => x.Indice))
            {
                string letra = Convert.ToChar(alternativa.Indice).ToString();
                var alternativaNova = new Alternativa
                {
                    Numero = alternativa.Numero,
                    Indice = alternativa.Indice,
                    Correta = alternativa.Correta,
                    Resposta = $"{letra.ToLower()}) {alternativa.Resposta}",
                    Questao_Numero = alternativa.Questao_Numero
                };
                listAlternativas.Items.Add(alternativaNova);
            }
        }

        private bool AlternativaEhValida()
        {
            var indiceAlternativa = BuscarIndiceAlternativa();

            if (txtResposta.Text.Trim() == "")
            {
                MessageBox.Show("Resposta deve ser cadastrada!", "Adicionando Alternativas",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }

            if (checkBoxAlternativaCorreta.Checked && _alternativas.Any(x => x.Correta && x.Indice != indiceAlternativa))
            {
                MessageBox.Show("Alternativa correta já está cadastrada!", "Adicionando Alternativas",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }

            if (_alternativas.Any(x => x.Resposta.ToUpper() == txtResposta.Text.ToUpper() && x.Indice != indiceAlternativa))
            {
                MessageBox.Show("Resposta já cadastrada em outra Alternativa!", "Adicionando Alternativas",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }

            return true;
        }

        private void listAlternativas_Click(object sender, EventArgs e)
        {
            Alternativa alternativaSelecionada = (Alternativa)listAlternativas.SelectedItem;

            if (alternativaSelecionada != null)
            {
                labelIdAlternativa.Text = alternativaSelecionada.Numero.ToString();
                labelLetraAlternativa.Text = alternativaSelecionada.Resposta.Substring(0, 1);
                txtResposta.Text = alternativaSelecionada.Resposta.Substring(3);
                checkBoxAlternativaCorreta.Checked = alternativaSelecionada.Correta;
            }

            btnAdicionarResposta.Enabled = true;
        }

        private void DefinirDisciplinaSelecionada()
        {
            var disciplina = _disciplinas.FirstOrDefault(x => x.Numero == _questaoObjetiva.Disciplina?.Numero);
            cmbDisciplinas.SelectedItem = disciplina;
        }

        private void DefinirMateriaSelecionada()
        {
            var materia = _materias.FirstOrDefault(x => x.Numero == _questaoObjetiva.Materia?.Numero);
            cmbMaterias.SelectedItem = materia;
        }
    }
}
