using FluentValidation;

namespace Ashraak.Cctv.Service.Application.Commands.RescheduleSchedule;

internal sealed class RescheduleScheduleCommandValidator : AbstractValidator<RescheduleScheduleCommand>
{
    public RescheduleScheduleCommandValidator()
    {
        RuleFor(x => x.ScheduleId).NotEmpty();
        RuleFor(x => x.NewScheduledDate).NotEmpty();
        RuleFor(x => x.Reason).NotEmpty().MinimumLength(10).MaximumLength(500);
        RuleFor(x => x.RowVersion).GreaterThan((uint)0);
    }
}
