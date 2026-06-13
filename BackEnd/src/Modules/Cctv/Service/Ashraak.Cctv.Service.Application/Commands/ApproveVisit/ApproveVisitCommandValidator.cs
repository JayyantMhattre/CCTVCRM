using FluentValidation;

namespace Ashraak.Cctv.Service.Application.Commands.ApproveVisit;

internal sealed class ApproveVisitCommandValidator : AbstractValidator<ApproveVisitCommand>
{
    public ApproveVisitCommandValidator()
    {
        RuleFor(x => x.VisitId).NotEmpty();
        RuleFor(x => x.ReviewRemarks).MaximumLength(1000);
    }
}
