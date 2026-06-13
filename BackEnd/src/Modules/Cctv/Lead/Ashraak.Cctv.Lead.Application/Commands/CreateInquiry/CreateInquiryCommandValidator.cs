using FluentValidation;

namespace Ashraak.Cctv.Lead.Application.Commands.CreateInquiry;

internal sealed class CreateInquiryCommandValidator : AbstractValidator<CreateInquiryCommand>
{
    public CreateInquiryCommandValidator()
    {
        RuleFor(x => x.InquiryType).NotEmpty().WithMessage("Inquiry type is required.");
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200).WithMessage("Name is required.");
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(320);
        RuleFor(x => x.Phone).NotEmpty().MaximumLength(32);
        RuleFor(x => x.City).NotEmpty().MaximumLength(100);
    }
}
