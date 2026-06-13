using FluentValidation;

namespace Ashraak.Cctv.Customer.Application.Commands.ChangeCustomerStatus;

internal sealed class ChangeCustomerStatusCommandValidator : AbstractValidator<ChangeCustomerStatusCommand>
{
    public ChangeCustomerStatusCommandValidator()
    {
        RuleFor(x => x.Status).NotEmpty();
    }
}
