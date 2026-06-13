using FluentValidation;

namespace Ashraak.Cctv.Invoice.Application.Commands.SendInvoice;

internal sealed class SendInvoiceCommandValidator : AbstractValidator<SendInvoiceCommand>
{
    public SendInvoiceCommandValidator()
    {
        RuleFor(x => x.InvoiceId).NotEmpty();
        RuleFor(x => x.RowVersion).GreaterThan((uint)0);
    }
}
