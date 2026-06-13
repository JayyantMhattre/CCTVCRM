using FluentValidation;

namespace Ashraak.Cctv.Amc.Application.Commands.PublishPlanVersion;

internal sealed class PublishPlanVersionCommandValidator : AbstractValidator<PublishPlanVersionCommand>
{
    public PublishPlanVersionCommandValidator()
    {
        RuleFor(x => x.PlanId).NotEmpty();
        RuleFor(x => x.VersionId).NotEmpty();
    }
}
