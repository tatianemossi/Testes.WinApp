using System.Collections.Generic;

namespace Testes.Dominio.ModuloQuestao
{
    public interface IRepositorioAlternativa
    {
        void DeletarAlternativasPelaNumeroQuestao(int numeroQuestao);
        void Inserir(Alternativa alternativa);
        List<Alternativa> SelecionarPorNumeroQuestao(int numero);
    }
}
