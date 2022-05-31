using FluentValidation;
using FluentValidation.Results;
using System.Collections.Generic;
using System.Linq;
using Testes.Dominio.ModuloQuestao;
using Testes.Dominio.ModuloTeste;
using Testes.Infra.Arquivos.Compartilhado;
using Testes.Infra.Arquivos.Compartilhado.Serializador;

namespace Testes.Infra.Arquivos.ModuloTeste
{
    public class RepositorioTesteEmArquivo : RepositorioEmArquivoBase<Teste>, IRepositorioTeste
    {
        public RepositorioTesteEmArquivo(DataContext dataContext) : base(dataContext)
        {

        }

        public ValidationResult Editar(Teste teste, List<QuestaoObjetiva> questoesSorteadas)
        {
            throw new System.NotImplementedException();
        }

        public ValidationResult Inserir(Teste teste, List<QuestaoObjetiva> questoesSorteadas)
        {
            throw new System.NotImplementedException();
        }

        public override List<Teste> ObterRegistros()
        {
            return dataContext.Testes;
        }

        public override AbstractValidator<Teste> ObterValidador()
        {
            return new ValidadorTeste();
        }
    }
}
