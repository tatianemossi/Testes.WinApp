using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System.Collections.Generic;
using System.Windows.Forms;
using Testes.Dominio.ModuloDisciplina;
using Testes.Dominio.ModuloMateria;
using Testes.Dominio.ModuloQuestao;
using Testes.Dominio.ModuloTeste;
using Testes.WinApp.Compartilhado;
using System;
using System.Windows.Forms;
using System.Diagnostics;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.Text;

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
            var testeSelecionado = ObtemTesteSelecionado();

            if (testeSelecionado == null)
            {
                MessageBox.Show("Selecione um Teste primeiro",
                "Edição de Testes", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            var questoes = _repositorioQuestao.SelecionarTodos();
            var disciplinas = _repositorioDisciplina.SelecionarTodos();
            var materias = _repositorioMateria.SelecionarTodos();

            TelaCadastroTestesForm tela = new TelaCadastroTestesForm(disciplinas, materias, questoes);

            tela.Teste = testeSelecionado;

            tela.GravarRegistro = _repositorioTeste.Editar;

            DialogResult resultado = tela.ShowDialog();

            if (resultado == DialogResult.OK)
            {
                CarregarTestes();
            }
        }

        public void Duplicar()
        {
            var testeSelecionado = ObtemTesteSelecionado();

            if (testeSelecionado == null)
            {
                MessageBox.Show("Selecione um Teste primeiro",
                "Duplicação de Testes", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            var questoes = _repositorioQuestao.SelecionarTodos();
            var disciplinas = _repositorioDisciplina.SelecionarTodos();
            var materias = _repositorioMateria.SelecionarTodos();

            TelaCadastroTestesForm tela = new TelaCadastroTestesForm(disciplinas, materias, questoes);

            tela.Teste = testeSelecionado.Clone();

            tela.GravarRegistro = _repositorioTeste.Inserir;

            DialogResult resultado = tela.ShowDialog();

            if (resultado == DialogResult.OK)
            {
                CarregarTestes();
            }
        }

        public void GerarPdf()
        {
            PdfDocument document = new PdfDocument();
            document.Info.Title = "Created with PDFsharp";

            PdfPage page = document.AddPage();

            XGraphics gfx = XGraphics.FromPdfPage(page);

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            XFont font = new XFont("Verdana", 20, XFontStyle.BoldItalic);

            gfx.DrawString("Hello, World!", font, XBrushes.Black,
              new XRect(0, 0, page.Width, page.Height),
              XStringFormats.Center);

            const string filename = @"C:\temp\HelloWorld.pdf";
            document.Save(filename);
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
