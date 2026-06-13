using FluentValidation;

namespace Ashraak.Cctv.Amc.Application.Commands.CreatePlanVersion;

internal sealed class CreatePlanVersionCommandValidator : AbstractValidator<CreatePlanVersionCommand>
{
    public CreatePlanVersionCommandValidator()
    {
        RuleFor(x => x.PlanId).NotEmpty();
        RuleFor(x => x.Price).GreaterThan(0);
        RuleFor(x => x.VisitFrequency).GreaterThan(0);
        RuleFor(x => x.SlaDescription).NotEmpty().MaximumLength(4000);
        RuleFor(x => x.IncludedServices).NotEmpty();
    }
}
