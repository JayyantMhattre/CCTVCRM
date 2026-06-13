using FluentValidation;

namespace Ashraak.Cctv.Engineer.Application.Commands.CreateEngineer;

internal sealed class CreateEngineerCommandValidator : AbstractValidator<CreateEngineerCommand>
{
    public CreateEngineerCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Phone).NotEmpty().MaximumLength(32);
    }
}
