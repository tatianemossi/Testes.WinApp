using System.Collections.Generic;
using System.Windows.Forms;
using Testes.Dominio.ModuloDisciplina;
using Testes.Dominio.ModuloMateria;
using Testes.WinApp.Compartilhado;

namespace Testes.WinApp.ModuloMateria
{
    public class ControladorMateria : ControladorBase
    {
        private readonly IRepositorioMateria _repositorioMateria;
        private readonly IRepositorioDisciplina _repositorioDisciplina;
        private TabelaMateriasControl _tabelaMaterias;

        public ControladorMateria(IRepositorioMateria repositorioMateria, IRepositorioDisciplina repositorioDisciplina)
        {
            this._repositorioMateria = repositorioMateria;
            this._repositorioDisciplina = repositorioDisciplina;
        }

        public override void Inserir()
        {
            var disciplinas = _repositorioDisciplina.SelecionarTodos();

            TelaCadastroMateriasForm tela = new TelaCadastroMateriasForm(disciplinas, this._repositorioMateria);
            tela.Materia = new Materia();

            tela.GravarRegistro = _repositorioMateria.Inserir;

            DialogResult resultado = tela.ShowDialog();

            if (resultado == DialogResult.OK)
            {
                CarregarMaterias();
            }
        }

        public override void Editar()
        {
            Materia materiaSelecionada = ObtemMateriaSelecionada();

            if (materiaSelecionada == null)
            {
                MessageBox.Show("Selecione uma Matéria primeiro",
                "Edição de Matérias", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            var disciplinas = _repositorioDisciplina.SelecionarTodos();

            TelaCadastroMateriasForm tela = new TelaCadastroMateriasForm(disciplinas, this._repositorioMateria);

            tela.Materia = materiaSelecionada;

            tela.GravarRegistro = _repositorioMateria.Editar;

            DialogResult resultado = tela.ShowDialog();

            if (resultado == DialogResult.OK)
            {
                CarregarMaterias();
            }
        }

        public override void Excluir()
        {
            Materia materiaSelecionada = ObtemMateriaSelecionada();

            if (materiaSelecionada == null)
            {
                MessageBox.Show("Selecione um Matéria primeiro",
                "Exclusão de Matérias", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            DialogResult resultado = MessageBox.Show("Deseja realmente excluir a Matéria?",
                "Exclusão de Matérias", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

            if (resultado == DialogResult.OK)
            {
                _repositorioMateria.Excluir(materiaSelecionada);
                CarregarMaterias();
            }
        }

        public override UserControl ObtemListagem()
        {
            if (_tabelaMaterias == null)
                _tabelaMaterias = new TabelaMateriasControl();

            CarregarMaterias();

            return _tabelaMaterias;
        }

        public override ConfiguracaoToolboxBase ObtemConfiguracaoToolbox()
        {
            return new ConfiguracaoToolboxMateria();
        }


        private Materia ObtemMateriaSelecionada()
        {
            var numero = _tabelaMaterias.ObtemNumeroMateriaSelecionada();

            return _repositorioMateria.SelecionarPorNumero(numero);
        }

        private void CarregarMaterias()
        {
            List<Materia> materias = _repositorioMateria.SelecionarTodos();

            CarregarDisciplinasDasMaterias(materias);

            _tabelaMaterias.AtualizarRegistros(materias);

            TelaPrincipalForm.Instancia.AtualizarRodape($"Visualizando {materias.Count} Matéria(s)");
        }

        private void CarregarDisciplinasDasMaterias(List<Materia> materias)
        {
            foreach (var materia in materias)
            {
                materia.Disciplina = _repositorioDisciplina.SelecionarPorNumero(materia.NumeroDisciplina);
            }
        }
    }
}