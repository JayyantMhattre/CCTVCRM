using Ashraak.Cctv.Invoice.Application.Mapping;
using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Contracts.CctvCrm.Enums;
using FluentValidation;

namespace Ashraak.Cctv.Invoice.Application.Commands.CreateInvoice;

internal sealed class CreateInvoiceCommandValidator : AbstractValidator<CreateInvoiceCommand>
{
    internal static readonly HashSet<string> ValidTypes =
    [
        InvoiceTypeContract.AmcRenewal,
        InvoiceTypeContract.NewAmc,
        InvoiceTypeContract.EmergencyService,
        InvoiceTypeContract.SpareReplacement,
        InvoiceTypeContract.AdditionalCharges,
        InvoiceTypeContract.Other
    ];

    public CreateInvoiceCommandValidator()
    {
        RuleFor(x => x.CustomerId).NotEmpty();
        RuleFor(x => x.InvoiceType)
            .NotEmpty()
            .Must(t => ValidTypes.Contains(t))
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
        RuleFor(x => x.TaxAmount).GreaterThanOrEqualTo(0).When(x => x.TaxAmount.HasValue);
    }

    private static Domain.Enums.InvoiceType ParseType(string value) =>
        InvoiceMapper.TryParseType(value, out var type) ? type : Domain.Enums.InvoiceType.Other;
}
