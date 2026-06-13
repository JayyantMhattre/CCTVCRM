using FluentValidation;

namespace Ashraak.Cctv.Service.Application.Commands.CreateAdHocSchedule;

internal sealed class CreateAdHocScheduleCommandValidator : AbstractValidator<CreateAdHocScheduleCommand>
{
    public CreateAdHocScheduleCommandValidator()
    {
        RuleFor(x => x.ContractTermId).NotEmpty();
        RuleFor(x => x.SiteId).NotEmpty();
        RuleFor(x => x.ScheduledDate).NotEmpty();
    }
}
