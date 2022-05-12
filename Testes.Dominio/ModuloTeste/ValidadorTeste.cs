using FluentValidation;

namespace Testes.Dominio.ModuloTeste
{
    public class ValidadorTeste : AbstractValidator<Teste>
    {
        public ValidadorTeste()
        {
            RuleFor(x => x.Titulo)
               .NotNull().NotEmpty();

            RuleFor(x => x.Data)
              .NotNull().NotEmpty();

            RuleFor(x => x.Disciplina)
               .NotNull().NotEmpty();

            RuleFor(x => x.Materia)
               .NotNull().NotEmpty();

            RuleFor(x => x.NumeroQuestoes)
               .NotNull().NotEmpty();
        }

    }
}
