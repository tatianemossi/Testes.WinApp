using FluentValidation;

namespace Testes.Dominio.ModuloQuestao
{
    public class ValidadorQuestaoObjetiva : AbstractValidator<QuestaoObjetiva>
    {
        public ValidadorQuestaoObjetiva()
        {
            RuleFor(x => x.Materia)
               .NotNull().NotEmpty();

            RuleFor(x => x.Bimestre)
               .NotNull().NotEmpty();

            RuleFor(x => x.Gabarito)
               .NotNull().NotEmpty();
        }
    }
}
