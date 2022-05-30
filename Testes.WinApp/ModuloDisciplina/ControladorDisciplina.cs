using System.Collections.Generic;
using System.Windows.Forms;
using Testes.Dominio.ModuloDisciplina;
using Testes.Dominio.ModuloMateria;
using Testes.WinApp.Compartilhado;

namespace Testes.WinApp.ModuloDisciplina
{
    public class ControladorDisciplina : ControladorBase
    {
        private readonly IRepositorioDisciplina _repositorioDisciplina;
        private readonly IRepositorioMateria _repositorioMateria;
        private TabelaDisiplinasControl _tabelaDisciplinas;

        public ControladorDisciplina(IRepositorioDisciplina repositorioDisciplina, IRepositorioMateria repositorioMateria)
        {
            _repositorioDisciplina = repositorioDisciplina;
            _repositorioMateria = repositorioMateria;
        }

        public override void Inserir()
        {
            TelaCadastroDisciplinasForm tela = new TelaCadastroDisciplinasForm(this._repositorioDisciplina);
            tela.Disciplina = new Disciplina();

            tela.GravarRegistro = _repositorioDisciplina.Inserir;

            DialogResult resultado = tela.ShowDialog();

            if (resultado == DialogResult.OK)
                CarregarDisciplinas();
        }

        public override void Editar()
        {
            Disciplina DisciplinaSelecionada = ObtemDisciplinaSelecionada();

            if (DisciplinaSelecionada == null)
            {
                MessageBox.Show("Selecione um Disciplina primeiro",
                "Edição de Disciplinas", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            TelaCadastroDisciplinasForm tela = new TelaCadastroDisciplinasForm(_repositorioDisciplina);

            tela.Disciplina = DisciplinaSelecionada;

            tela.GravarRegistro = _repositorioDisciplina.Editar;

            DialogResult resultado = tela.ShowDialog();

            if (resultado == DialogResult.OK)
            {
                CarregarDisciplinas();
            }
        }

        public override void Excluir()
        {
            Disciplina disciplinaSelecionada = ObtemDisciplinaSelecionada();

            if (disciplinaSelecionada == null)
            {
                MessageBox.Show("Selecione um Disciplina primeiro",
                "Exclusão de Disciplinas", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (_repositorioMateria.ExisteMateriaPeloNumeroDisciplina(disciplinaSelecionada.Numero))
            {
                MessageBox.Show("Não é possível excluir! A Disciplina está relacionada com uma matéria.",
               "Exclusão de Disciplinas", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            DialogResult resultado = MessageBox.Show("Deseja realmente excluir a Disciplina?",
                "Exclusão de Disciplinas", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

            if (resultado == DialogResult.OK)
            {
                _repositorioDisciplina.Excluir(disciplinaSelecionada);
                CarregarDisciplinas();
            }
        }

        public override UserControl ObtemListagem()
        {
            if (_tabelaDisciplinas == null)
                _tabelaDisciplinas = new TabelaDisiplinasControl();

            CarregarDisciplinas();

            return _tabelaDisciplinas;
        }

        public override ConfiguracaoToolboxBase ObtemConfiguracaoToolbox()
        {
            return new ConfiguracaoToolboxDisciplina();
        }

        private Disciplina ObtemDisciplinaSelecionada()
        {
            var numero = _tabelaDisciplinas.ObtemNumeroDisciplinaSelecionada();

            return _repositorioDisciplina.SelecionarPorNumero(numero);
        }

        private void CarregarDisciplinas()
        {
            List<Disciplina> Disciplinas = _repositorioDisciplina.SelecionarTodos();

            _tabelaDisciplinas.AtualizarRegistros(Disciplinas);

            TelaPrincipalForm.Instancia.AtualizarRodape($"Visualizando {Disciplinas.Count} Disciplina(s)");
        }
    }
}