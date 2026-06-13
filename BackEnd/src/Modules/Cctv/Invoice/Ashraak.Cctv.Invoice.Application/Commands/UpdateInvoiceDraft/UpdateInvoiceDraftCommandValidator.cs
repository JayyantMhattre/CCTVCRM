using Ashraak.Cctv.Invoice.Application.Commands.CreateInvoice;
using Ashraak.Cctv.Invoice.Application.Mapping;
using FluentValidation;

namespace Ashraak.Cctv.Invoice.Application.Commands.UpdateInvoiceDraft;

internal sealed class UpdateInvoiceDraftCommandValidator : AbstractValidator<UpdateInvoiceDraftCommand>
{
    public UpdateInvoiceDraftCommandValidator()
    {
        RuleFor(x => x.InvoiceId).NotEmpty();
        RuleFor(x => x.RowVersion).GreaterThan((uint)0);
        Include(new CreateInvoiceCommandValidatorAdapter());
    }

    private sealed class CreateInvoiceCommandValidatorAdapter : AbstractValidator<UpdateInvoiceDraftCommand>
    {
        public CreateInvoiceCommandValidatorAdapter()
        {
            RuleFor(x => x.InvoiceType)
                .NotEmpty()
                .Must(t => CreateInvoiceCommandValidator.ValidTypes.Contains(t))
                .WithMessage("Invalid invoice type.");
            RuleFor(x => x.InvoiceDate).NotEmpty();
            RuleFor(x => x.Lines).NotEmpty().WithMessage("At least one line item is required.");
            RuleForEach(x => x.Lines).ChildRules(line =>
            {
                line.RuleFor(l => l.Description).NotEmpty().MaximumLength(500);
                line.RuleFor(l => l.Quantity).GreaterThan(0);
                line.RuleFor(l => l.UnitPrice).GreaterThanOrEqualTo(0);
            });
            RuleFor(x => x)
                .Must(x => !InvoiceMapper.RequiresAmcTerm(ParseType(x.InvoiceType)) || x.AmcContractTermId.HasValue)
                .WithMessage("AMC contract term is required for AmcRenewal and NewAmc invoice types.");
        }

        private static Domain.Enums.InvoiceType ParseType(string value) =>
            InvoiceMapper.TryParseType(value, out var type) ? type : Domain.Enums.InvoiceType.Other;
    }
}
