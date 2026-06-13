using FluentValidation;

namespace Ashraak.Cctv.Invoice.Application.Commands.GenerateInvoice;

internal sealed class GenerateInvoiceCommandValidator : AbstractValidator<GenerateInvoiceCommand>
{
    public GenerateInvoiceCommandValidator()
    {
        RuleFor(x => x.InvoiceId).NotEmpty();
        RuleFor(x => x.RowVersion).GreaterThan((uint)0);
    }
}
