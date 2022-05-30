using System.Collections.Generic;
using System.Windows.Forms;
using Testes.Dominio.ModuloDisciplina;
using Testes.Dominio.ModuloMateria;
using Testes.Dominio.ModuloQuestao;
using Testes.WinApp.Compartilhado;

namespace Testes.WinApp.ModuloQuestao
{
    public class ControladorQuestao : ControladorBase
    {
        private readonly IRepositorioQuestaoObjetiva _repositorioQuestao;
        private readonly IRepositorioMateria _repositorioMateria;
        private readonly IRepositorioDisciplina _repositorioDisciplina;
        private readonly IRepositorioAlternativa _repositorioAlternativa;
        private TabelaQuestoesControl tabelaQuestoes;

        public ControladorQuestao(IRepositorioQuestaoObjetiva repositorioQuestao, IRepositorioMateria repositorioMateria,
            IRepositorioDisciplina repositorioDisciplina, IRepositorioAlternativa repositorioAlternativa)
        {
            _repositorioQuestao = repositorioQuestao;
            _repositorioMateria = repositorioMateria;
            _repositorioDisciplina = repositorioDisciplina;
            _repositorioAlternativa = repositorioAlternativa;
        }

        public override void Inserir()
        {
            var disciplinas = _repositorioDisciplina.SelecionarTodos();
            var materias = _repositorioMateria.SelecionarTodos();

            TelaCadastroQuestoesForm tela = new TelaCadastroQuestoesForm(disciplinas, materias, _repositorioAlternativa);
            tela.QuestaoObjetiva = new QuestaoObjetiva();

            tela.GravarRegistro = _repositorioQuestao.Inserir;

            DialogResult resultado = tela.ShowDialog();

            if (resultado == DialogResult.OK)
            {
                CarregarQuestoes();
            }
        }

        public override void Editar()
        {
            var QuestaoSelecionada = ObtemQuestaoSelecionada();

            if (QuestaoSelecionada == null)
            {
                MessageBox.Show("Selecione uma Questão primeiro",
                "Edição de Questões", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            var disciplinas = _repositorioDisciplina.SelecionarTodos();
            var materias = _repositorioMateria.SelecionarTodos();

            TelaCadastroQuestoesForm tela = new TelaCadastroQuestoesForm(disciplinas, materias, _repositorioAlternativa);

            tela.QuestaoObjetiva = QuestaoSelecionada;

            tela.GravarRegistro = _repositorioQuestao.Editar;

            DialogResult resultado = tela.ShowDialog();

            if (resultado == DialogResult.OK)
            {
                CarregarQuestoes();
            }
        }

        public override void Excluir()
        {
            QuestaoObjetiva QuestaoSelecionada = ObtemQuestaoSelecionada();

            if (QuestaoSelecionada == null)
            {
                MessageBox.Show("Selecione uma Questão primeiro",
                "Exclusão de Questões", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            DialogResult resultado = MessageBox.Show("Deseja realmente excluir a Questão?",
                "Exclusão de Questões", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

            if (resultado == DialogResult.OK)
            {
                _repositorioQuestao.Excluir(QuestaoSelecionada);
                CarregarQuestoes();
            }
        }

        public override UserControl ObtemListagem()
        {
            if (tabelaQuestoes == null)
                tabelaQuestoes = new TabelaQuestoesControl();

            CarregarQuestoes();

            return tabelaQuestoes;
        }

        public override ConfiguracaoToolboxBase ObtemConfiguracaoToolbox()
        {
            return new ConfiguracaoToolboxQuestao();
        }

        private QuestaoObjetiva ObtemQuestaoSelecionada()
        {
            var numero = tabelaQuestoes.ObtemNumeroQuestaoSelecionada();

            var questao = _repositorioQuestao.SelecionarPorNumero(numero);

            questao.Alternativas = _repositorioAlternativa.SelecionarPorNumeroQuestao(numero);

            return questao;
        }

        private void CarregarQuestoes()
        {
            List<QuestaoObjetiva> questoes = _repositorioQuestao.SelecionarTodos();

            tabelaQuestoes.AtualizarRegistros(questoes);

            TelaPrincipalForm.Instancia.AtualizarRodape($"Visualizando {questoes.Count} Questão(ões)");
        }
    }
}