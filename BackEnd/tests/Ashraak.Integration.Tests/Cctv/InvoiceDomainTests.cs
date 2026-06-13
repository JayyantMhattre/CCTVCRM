using Ashraak.Cctv.Invoice.Application.Abstractions;
using Ashraak.Cctv.Invoice.Domain.Aggregates.Invoice;
using Ashraak.Cctv.Invoice.Domain.Enums;
using Ashraak.Cctv.Invoice.Domain.Repositories;
using Ashraak.Cctv.Invoice.Infrastructure.Services;
using FluentAssertions;
using NSubstitute;
using Xunit;
using InvoiceAggregate = Ashraak.Cctv.Invoice.Domain.Aggregates.Invoice.Invoice;

namespace Ashraak.Integration.Tests.Cctv;

/// <summary>D1-7 domain tests for Option B invoicing rules and lifecycle.</summary>
public sealed class InvoiceDomainTests
{
    private static readonly Guid TestUserId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private static readonly Guid AmcTermId = Guid.Parse("22222222-2222-2222-2222-222222222222");

    [Fact]
    public void Create_AmcRenewal_RequiresAmcTerm()
    {
        var act = () => CreateInvoice(InvoiceType.AmcRenewal, autoFillAmcTerm: false);
        act.Should().Throw<InvalidOperationException>().WithMessage("*term*");
    }

    [Fact]
    public void Create_EmergencyService_AllowsOptionalAmcTerm()
    {
        var invoice = CreateInvoice(InvoiceType.EmergencyService, amcTermId: null);
        invoice.AmcContractTermId.Should().BeNull();
        invoice.Status.Should().Be(InvoiceStatus.Draft);
    }

    [Fact]
    public void UpdateDraft_OnlyWhenDraft()
    {
        var invoice = CreateGeneratedInvoice();

        var act = () => invoice.UpdateDraft(
            null,
            InvoiceType.EmergencyService,
            null,
            null,
            null,
            DateOnly.FromDateTime(DateTime.UtcNow),
            null,
            [new InvoiceAggregate.LineDraft("Updated", 1, 100m, 1)],
            0m,
            TestUserId);

        act.Should().Throw<InvalidOperationException>().WithMessage("*Draft*");
    }

    [Fact]
    public void LineTotals_ComputedCorrectly()
    {
        var invoice = CreateInvoice(
            InvoiceType.EmergencyService,
            [new InvoiceAggregate.LineDraft("Service call", 2, 150.50m, 1)]);

        invoice.SubtotalAmount.Should().Be(301.00m);
        invoice.TotalAmount.Should().Be(invoice.SubtotalAmount + invoice.TaxAmount);
        invoice.Lines.Single().LineTotal.Should().Be(301.00m);
    }

    [Fact]
    public void Generate_DraftToGenerated_LinksPdfAttachment()
    {
        var invoice = CreateInvoice(InvoiceType.EmergencyService);
        var pdfFileId = Guid.NewGuid();

        invoice.Generate(pdfFileId, TestUserId);

        invoice.Status.Should().Be(InvoiceStatus.Generated);
        invoice.InvoicePdfAttachment.Should().NotBeNull();
        invoice.InvoicePdfAttachment!.FileId.Should().Be(pdfFileId);
    }

    [Fact]
    public void Send_GeneratedToSent()
    {
        var invoice = CreateGeneratedInvoice();

        invoice.Send(TestUserId);

        invoice.Status.Should().Be(InvoiceStatus.Sent);
    }

    [Fact]
    public void MarkPaid_SentToPaid()
    {
        var invoice = CreateSentInvoice();
        var paidAt = DateTime.UtcNow;

        invoice.MarkPaid(paidAt, TestUserId);

        invoice.Status.Should().Be(InvoiceStatus.Paid);
        invoice.PaidAtUtc.Should().Be(paidAt);
    }

    [Fact]
    public void Cancel_FromDraft_WithReason()
    {
        var invoice = CreateInvoice(InvoiceType.EmergencyService);

        invoice.Cancel("Customer requested cancellation.", TestUserId);

        invoice.Status.Should().Be(InvoiceStatus.Cancelled);
    }

    [Fact]
    public async Task InvoiceNumberGenerator_ProducesExpectedFormat()
    {
        var repository = Substitute.For<IInvoiceRepository>();
        repository.GetYearlySequenceAsync(2026, Arg.Any<CancellationToken>()).Returns(12);

        var generator = new InvoiceNumberGenerator(repository);
        var number = await generator.GenerateNextAsync(CancellationToken.None);

        number.Should().Be("INV-2026-0013");
    }

    private static InvoiceAggregate CreateInvoice(
        InvoiceType type,
        IReadOnlyList<InvoiceAggregate.LineDraft>? lines = null,
        Guid? amcTermId = null,
        bool autoFillAmcTerm = true)
    {
        var resolvedAmcTermId = amcTermId;
        if (resolvedAmcTermId is null && autoFillAmcTerm && type is InvoiceType.AmcRenewal or InvoiceType.NewAmc)
            resolvedAmcTermId = AmcTermId;

        return InvoiceAggregate.Create(
            InvoiceId.New(),
            "INV-2026-0001",
            Guid.NewGuid(),
            null,
            type,
            resolvedAmcTermId,
            null,
            null,
            DateOnly.FromDateTime(DateTime.UtcNow),
            null,
            lines ?? [new InvoiceAggregate.LineDraft("Line item", 1, 100m, 1)],
            0m,
            TestUserId);
    }

    private static InvoiceAggregate CreateGeneratedInvoice()
    {
        var invoice = CreateInvoice(InvoiceType.EmergencyService);
        invoice.Generate(Guid.NewGuid(), TestUserId);
        return invoice;
    }

    private static InvoiceAggregate CreateSentInvoice()
    {
        var invoice = CreateGeneratedInvoice();
        invoice.Send(TestUserId);
        return invoice;
    }
}
