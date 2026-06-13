using FluentValidation;

namespace Ashraak.Cctv.Service.Application.Commands.UpdateVisitRemarks;

internal sealed class UpdateVisitRemarksCommandValidator : AbstractValidator<UpdateVisitRemarksCommand>
{
    public UpdateVisitRemarksCommandValidator()
    {
        RuleFor(x => x.VisitId).NotEmpty();
        RuleFor(x => x.Remarks).MaximumLength(4000);
        RuleFor(x => x.RowVersion).GreaterThan((uint)0);
    }
}
