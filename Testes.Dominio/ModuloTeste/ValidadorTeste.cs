using FluentValidation;

namespace Testes.Dominio.ModuloTeste
{
    public class ValidadorTeste : AbstractValidator<Teste>
    {
        public ValidadorTeste()
        {
            RuleFor(x => x.NumeroQuestoes)
               .NotNull().NotEmpty();

            RuleFor(x => x.Data)
               .NotNull().NotEmpty();

            RuleFor(x => x.Disciplina)
               .NotNull().NotEmpty();

            RuleFor(x => x.QuestoesObjetivas)
               .NotNull().NotEmpty();
        }

    }
}
