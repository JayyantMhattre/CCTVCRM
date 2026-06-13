using FluentValidation;

namespace Ashraak.Cctv.Amc.Application.Commands.RetirePlan;

internal sealed class RetirePlanCommandValidator : AbstractValidator<RetirePlanCommand>
{
    public RetirePlanCommandValidator()
    {
        RuleFor(x => x.PlanId).NotEmpty();
    }
}
