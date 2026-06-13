using FluentValidation;

namespace Ashraak.Cctv.Customer.Application.Commands.ChangeSiteStatus;

internal sealed class ChangeSiteStatusCommandValidator : AbstractValidator<ChangeSiteStatusCommand>
{
    public ChangeSiteStatusCommandValidator()
    {
        RuleFor(x => x.SiteId).NotEmpty();
        RuleFor(x => x.Status).NotEmpty().Must(s => s is "Active" or "Inactive")
            .WithMessage("Status must be Active or Inactive.");
        RuleFor(x => x.RowVersion).GreaterThan((uint)0);
    }
}
