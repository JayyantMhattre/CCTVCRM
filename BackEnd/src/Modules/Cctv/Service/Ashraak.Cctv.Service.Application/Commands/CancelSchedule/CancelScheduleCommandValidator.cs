using FluentValidation;

namespace Ashraak.Cctv.Service.Application.Commands.CancelSchedule;

internal sealed class CancelScheduleCommandValidator : AbstractValidator<CancelScheduleCommand>
{
    public CancelScheduleCommandValidator()
    {
        RuleFor(x => x.ScheduleId).NotEmpty();
        RuleFor(x => x.Reason).NotEmpty().MaximumLength(500);
        RuleFor(x => x.RowVersion).GreaterThan((uint)0);
    }
}
