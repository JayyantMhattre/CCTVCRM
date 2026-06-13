using FluentValidation;

namespace Ashraak.Cctv.Invoice.Application.Commands.MarkInvoicePaid;

internal sealed class MarkInvoicePaidCommandValidator : AbstractValidator<MarkInvoicePaidCommand>
{
    public MarkInvoicePaidCommandValidator()
    {
        RuleFor(x => x.InvoiceId).NotEmpty();
        RuleFor(x => x.RowVersion).GreaterThan((uint)0);
    }
}
