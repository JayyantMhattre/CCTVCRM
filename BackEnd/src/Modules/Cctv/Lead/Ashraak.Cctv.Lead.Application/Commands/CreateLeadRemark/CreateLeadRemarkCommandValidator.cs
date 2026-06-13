using FluentValidation;

namespace Ashraak.Cctv.Lead.Application.Commands.CreateLeadRemark;

internal sealed class CreateLeadRemarkCommandValidator : AbstractValidator<CreateLeadRemarkCommand>
{
    public CreateLeadRemarkCommandValidator()
    {
        RuleFor(x => x.LeadId).NotEmpty();
        RuleFor(x => x.Text).NotEmpty().MaximumLength(4000);
    }
}
