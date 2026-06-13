using FluentValidation;

namespace Ashraak.Cctv.Lead.Application.Commands.ConvertLead;

internal sealed class ConvertLeadCommandValidator : AbstractValidator<ConvertLeadCommand>
{
    public ConvertLeadCommandValidator()
    {
        RuleFor(x => x.LeadId).NotEmpty();
        RuleFor(x => x.PlanVersionId).NotEmpty();
        RuleFor(x => x.SiteName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.SiteAddress).NotEmpty().MaximumLength(500);
        RuleFor(x => x.InitialTermEndDate)
            .GreaterThanOrEqualTo(x => x.InitialTermStartDate)
            .WithMessage("Term end date must be on or after start date.");
    }
}
