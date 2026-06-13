using FluentValidation;

namespace Ashraak.Cctv.Service.Application.Commands.StartVisit;

internal sealed class StartVisitCommandValidator : AbstractValidator<StartVisitCommand>
{
    public StartVisitCommandValidator()
    {
        RuleFor(x => x.VisitId).NotEmpty();
        RuleFor(x => x.RowVersion).GreaterThan((uint)0);
    }
}
