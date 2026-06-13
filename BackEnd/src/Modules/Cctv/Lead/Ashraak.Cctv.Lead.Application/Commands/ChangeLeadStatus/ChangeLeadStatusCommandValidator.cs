using FluentValidation;

namespace Ashraak.Cctv.Lead.Application.Commands.ChangeLeadStatus;

internal sealed class ChangeLeadStatusCommandValidator : AbstractValidator<ChangeLeadStatusCommand>
{
    public ChangeLeadStatusCommandValidator()
    {
        RuleFor(x => x.LeadId).NotEmpty();
        RuleFor(x => x.ToStatus).NotEmpty();
    }
}
