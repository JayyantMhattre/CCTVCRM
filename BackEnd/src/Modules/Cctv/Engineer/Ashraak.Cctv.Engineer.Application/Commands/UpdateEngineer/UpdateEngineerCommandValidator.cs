using FluentValidation;

namespace Ashraak.Cctv.Engineer.Application.Commands.UpdateEngineer;

internal sealed class UpdateEngineerCommandValidator : AbstractValidator<UpdateEngineerCommand>
{
    public UpdateEngineerCommandValidator()
    {
        RuleFor(x => x.EngineerId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Phone).NotEmpty().MaximumLength(32);
    }
}
