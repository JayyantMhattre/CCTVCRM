using FluentValidation;

namespace Ashraak.Cctv.Engineer.Application.Commands.ChangeEngineerStatus;

internal sealed class ChangeEngineerStatusCommandValidator : AbstractValidator<ChangeEngineerStatusCommand>
{
    public ChangeEngineerStatusCommandValidator()
    {
        RuleFor(x => x.EngineerId).NotEmpty();
        RuleFor(x => x.Status).NotEmpty();
    }
}
