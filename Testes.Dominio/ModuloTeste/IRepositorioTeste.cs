using FluentValidation.Results;
using System.Collections.Generic;
using Testes.Dominio.Compartilhado;
using Testes.Dominio.ModuloQuestao;

namespace Testes.Dominio.ModuloTeste
{
    public interface IRepositorioTeste : IRepositorio<Teste>
    {
        List<Teste> SelecionarTodos();

        ValidationResult Inserir(Teste teste, List<QuestaoObjetiva> questoesSorteadas);

        ValidationResult Editar(Teste teste, List<QuestaoObjetiva> questoesSorteadas);
    }
}
