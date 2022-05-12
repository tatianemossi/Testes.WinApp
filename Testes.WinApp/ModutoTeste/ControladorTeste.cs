using System.Collections.Generic;
using System.Windows.Forms;
using Testes.Dominio.ModuloDisciplina;
using Testes.Dominio.ModuloMateria;
using Testes.Dominio.ModuloQuestao;
using Testes.Dominio.ModuloTeste;
using Testes.WinApp.Compartilhado;

namespace Testes.WinApp.ModutoTeste
{
    public class ControladorTeste : ControladorBase
    {
        private readonly IRepositorioTeste _repositorioTeste;
        private readonly IRepositorioQuestaoObjetiva _repositorioQuestao;
        private readonly IRepositorioMateria _repositorioMateria;
        private readonly IRepositorioDisciplina _repositorioDisciplina;
        private TabelaTestesControl tabelaTestes;

        public ControladorTeste(IRepositorioTeste repositorioTeste, IRepositorioQuestaoObjetiva repositorioQuestao, 
            IRepositorioMateria repositorioMateria, IRepositorioDisciplina repositorioDisciplina)
        {
            _repositorioTeste = repositorioTeste;
            _repositorioQuestao = repositorioQuestao;
            _repositorioMateria = repositorioMateria;
            _repositorioDisciplina = repositorioDisciplina;
        }

        public override void Inserir()
        {
            var questoes = _repositorioQuestao.SelecionarTodos();
            var disciplinas = _repositorioDisciplina.SelecionarTodos();
            var materias = _repositorioMateria.SelecionarTodos();

            TelaCadastroTestesForm tela = new TelaCadastroTestesForm(disciplinas, materias, questoes);
            tela.Teste = new Teste();

            tela.GravarRegistro = _repositorioTeste.Inserir;

            DialogResult resultado = tela.ShowDialog();

            if (resultado == DialogResult.OK)
            {
                CarregarTestes();
            }
        }

        public override void Editar()
        {
            var TesteSelecionado = ObtemTesteSelecionado();

            if (TesteSelecionado == null)
            {
                MessageBox.Show("Selecione um Teste primeiro",
                "Edição de Testes", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            var questoes = _repositorioQuestao.SelecionarTodos();
            var disciplinas = _repositorioDisciplina.SelecionarTodos();
            var materias = _repositorioMateria.SelecionarTodos();

            TelaCadastroTestesForm tela = new TelaCadastroTestesForm(disciplinas, materias, questoes);

            tela.Teste = TesteSelecionado;

            tela.GravarRegistro = _repositorioTeste.Editar;

            DialogResult resultado = tela.ShowDialog();

            if (resultado == DialogResult.OK)
            {
                CarregarTestes();
            }
        }

        public override void Excluir()
        {
            Teste TesteSelecionado = ObtemTesteSelecionado();

            if (TesteSelecionado == null)
            {
                MessageBox.Show("Selecione um Teste primeiro",
                "Exclusão de Testes", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            DialogResult resultado = MessageBox.Show("Deseja realmente excluir o Teste?",
                "Exclusão de Testes", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

            if (resultado == DialogResult.OK)
            {
                _repositorioTeste.Excluir(TesteSelecionado);
                CarregarTestes();
            }
        }

        public override UserControl ObtemListagem()
        {
            if (tabelaTestes == null)
                tabelaTestes = new TabelaTestesControl();

            CarregarTestes();

            return tabelaTestes;
        }

        public override ConfiguracaoToolboxBase ObtemConfiguracaoToolbox()
        {
            return new ConfiguracaoToolboxTeste();
        }


        private Teste ObtemTesteSelecionado()
        {
            var numero = tabelaTestes.ObtemNumeroTesteSelecionado();

            return _repositorioTeste.SelecionarPorNumero(numero);
        }

        private void CarregarTestes()
        {
            List<Teste> testes = _repositorioTeste.SelecionarTodos();

            tabelaTestes.AtualizarRegistros(testes);

            TelaPrincipalForm.Instancia.AtualizarRodape($"Visualizando {testes.Count} Teste(s)");
        }
    }
}
