using FluentValidation;

namespace Ashraak.Cctv.Service.Application.Commands.ReturnVisit;

internal sealed class ReturnVisitCommandValidator : AbstractValidator<ReturnVisitCommand>
{
    public ReturnVisitCommandValidator()
    {
        RuleFor(x => x.VisitId).NotEmpty();
        RuleFor(x => x.ReturnReason).NotEmpty().MaximumLength(1000);
    }
}
