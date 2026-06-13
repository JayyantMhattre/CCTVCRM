using FluentValidation;

namespace Ashraak.Cctv.Service.Application.Commands.SubmitVisitReport;

internal sealed class SubmitVisitReportCommandValidator : AbstractValidator<SubmitVisitReportCommand>
{
    public SubmitVisitReportCommandValidator()
    {
        RuleFor(x => x.VisitId).NotEmpty();
        RuleFor(x => x.RowVersion).GreaterThan((uint)0);
        RuleFor(x => x.ClientCorrelationId).MaximumLength(100);
    }
}
