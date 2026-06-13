using FluentValidation;

namespace Ashraak.Cctv.Invoice.Application.Commands.CancelInvoice;

internal sealed class CancelInvoiceCommandValidator : AbstractValidator<CancelInvoiceCommand>
{
    public CancelInvoiceCommandValidator()
    {
        RuleFor(x => x.InvoiceId).NotEmpty();
        RuleFor(x => x.Reason).NotEmpty().MaximumLength(500);
        RuleFor(x => x.RowVersion).GreaterThan((uint)0);
    }
}
