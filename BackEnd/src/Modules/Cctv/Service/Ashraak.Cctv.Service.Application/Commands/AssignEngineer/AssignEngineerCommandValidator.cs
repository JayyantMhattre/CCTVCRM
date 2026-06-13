using FluentValidation;

namespace Ashraak.Cctv.Service.Application.Commands.AssignEngineer;

internal sealed class AssignEngineerCommandValidator : AbstractValidator<AssignEngineerCommand>
{
    public AssignEngineerCommandValidator()
    {
        RuleFor(x => x.ScheduleId).NotEmpty();
        RuleFor(x => x.EngineerId).NotEmpty();
        RuleFor(x => x.RowVersion).GreaterThan((uint)0);
    }
}
