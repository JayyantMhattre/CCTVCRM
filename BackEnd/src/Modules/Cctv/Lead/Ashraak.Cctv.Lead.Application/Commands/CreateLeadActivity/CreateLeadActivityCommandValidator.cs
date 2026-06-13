using FluentValidation;

namespace Ashraak.Cctv.Lead.Application.Commands.CreateLeadActivity;

internal sealed class CreateLeadActivityCommandValidator : AbstractValidator<CreateLeadActivityCommand>
{
    public CreateLeadActivityCommandValidator()
    {
        RuleFor(x => x.LeadId).NotEmpty();
        RuleFor(x => x.ActivityType).NotEmpty();
        RuleFor(x => x.Description).NotEmpty().MaximumLength(4000);
    }
}
