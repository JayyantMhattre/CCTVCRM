using Ashraak.Cctv.Amc.Domain.Aggregates.Contract;
using Ashraak.Cctv.Amc.Domain.Aggregates.Contract.Events;
using Ashraak.Cctv.Amc.Domain.Enums;
using Ashraak.Cctv.Amc.Domain.Repositories;
using Ashraak.Cctv.Integration.Application;
using Ashraak.Cctv.Integration.Application.Abstractions;
using Ashraak.Cctv.Service.Domain.Aggregates.Visit;
using Ashraak.Cctv.Service.Domain.Aggregates.Visit.Events;
using Ashraak.Cctv.Service.Domain.Enums;
using Ashraak.Cctv.Service.Domain.Repositories;
using Ashraak.SharedKernel.Contracts.CctvCrm;
using Ashraak.SharedKernel.Contracts.CctvCrm.Interfaces;
using Ashraak.SharedKernel.Contracts.FeatureFlags.Interfaces;
using Ashraak.Cctv.Amc.Infrastructure.Persistence;
using Ashraak.Cctv.Service.Infrastructure.Persistence;
using Ashraak.SharedKernel.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Ashraak.Cctv.Integration.Infrastructure.Services.PdfHandlers;

internal sealed class TermActivatedContractPdfHandler(
    IAmcContractRepository contractRepository,
    IAmcContractLookupService contractLookup,
    ICustomerLookupService customers,
    ISiteLookupService sites,
    IAmcPlanLookupService plans,
    IPdfGenerationService pdfGenerationService,
    ICctvFileStore fileStore,
    IFeatureFlagService featureFlags,
    ITenantContext tenantContext,
    IOptions<CctvNotificationOptions> options,
    AmcDbContext unitOfWork,
    ILogger<TermActivatedContractPdfHandler> logger) : INotificationHandler<TermActivatedDomainEvent>
{
    public async Task Handle(TermActivatedDomainEvent notification, CancellationToken cancellationToken)
    {
        if (!await featureFlags.IsEnabledAsync(CctvFeatureFlags.PdfEnabled, cancellationToken: cancellationToken))
            return;

        var contract = await contractRepository.GetByIdAsync(
            AmcContractId.From(notification.ContractId),
            cancellationToken);
        if (contract is null)
            return;

        var termId = AmcContractTermId.From(notification.TermId);
        if (contract.Documents.Any(d =>
                d.DocumentType == ContractDocumentType.ContractPdf && d.TermId == termId))
            return;

        var termDetail = await contractLookup.GetTermByIdAsync(
            notification.ContractId,
            notification.TermId,
            cancellationToken);
        var customer = await customers.GetCustomerAsync(contract.CustomerId, cancellationToken);
        var site = await sites.GetSiteAsync(contract.SiteId, cancellationToken);
        var plan = termDetail is not null
            ? await plans.GetPublishedVersionAsync(termDetail.PlanVersionId, cancellationToken)
            : null;

        var tenantId = CctvTenantHelper.ResolveTenantId(tenantContext, options.Value);
        var model = new
        {
            contract.ContractNumber,
            CustomerName = customer?.Name,
            SiteName = site?.Name,
            SiteAddress = site?.Address,
            PlanName = plan?.PlanName,
            PlanCode = plan?.PlanCode ?? termDetail?.PlanCode,
            StartDate = termDetail?.StartDate.ToString("yyyy-MM-dd"),
            EndDate = termDetail?.EndDate.ToString("yyyy-MM-dd"),
            termDetail?.AgreedPrice,
            plan?.VisitFrequencyPerYear,
            SlaTerms = plan?.SlaTerms,
            IncludedServices = plan is null ? string.Empty : string.Join(", ", plan.IncludedServices),
            ApprovedBy = notification.ActivatedBy.ToString(),
            ApprovedAtUtc = DateTime.UtcNow.ToString("u")
        };

        var bytes = await pdfGenerationService.GenerateAsync("contract", model, cancellationToken);
        var fileName = $"{contract.ContractNumber}-term-{notification.TermNo}.pdf";
        var fileId = await fileStore.StorePdfAsync(
            tenantId,
            notification.ActivatedBy,
            fileName,
            bytes,
            cancellationToken);

        contract.LinkDocument(
            AmcContractDocumentId.New(),
            fileId,
            ContractDocumentType.ContractPdf,
            $"AMC Contract — Term {notification.TermNo}",
            termId,
            notification.ActivatedBy);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        logger.LogInformation(
            "Generated contract PDF for {ContractNumber} term {TermNo} (file {FileId})",
            contract.ContractNumber,
            notification.TermNo,
            fileId);
    }
}

internal sealed class VisitReportApprovedPdfHandler(
    IServiceVisitRepository visitRepository,
    IVisitLookupService visitLookup,
    IScheduleLookupService scheduleLookup,
    IEngineerLookupService engineerLookup,
    ISiteLookupService siteLookup,
    IPdfGenerationService pdfGenerationService,
    ICctvFileStore fileStore,
    IFeatureFlagService featureFlags,
    ITenantContext tenantContext,
    IOptions<CctvNotificationOptions> options,
    ServiceDbContext unitOfWork,
    ILogger<VisitReportApprovedPdfHandler> logger) : INotificationHandler<VisitReportApprovedDomainEvent>
{
    public async Task Handle(VisitReportApprovedDomainEvent notification, CancellationToken cancellationToken)
    {
        if (!await featureFlags.IsEnabledAsync(CctvFeatureFlags.PdfEnabled, cancellationToken: cancellationToken))
            return;

        var visit = await visitRepository.GetByIdAsync(
            ServiceVisitId.From(notification.VisitId),
            cancellationToken);
        if (visit is null)
            return;

        if (visit.Attachments.Any(a => a.AttachmentType == VisitAttachmentType.ReportPdf))
            return;

        var detail = await visitLookup.GetVisitAsync(notification.VisitId, cancellationToken);
        var schedule = await scheduleLookup.GetScheduleAsync(notification.ScheduleId, cancellationToken);
        var engineer = await engineerLookup.GetAsync(visit.EngineerId, cancellationToken);
        var site = schedule is not null
            ? await siteLookup.GetSiteAsync(schedule.SiteId, cancellationToken)
            : null;

        var gpsSummary = detail?.Location is null
            ? "Not captured"
            : $"{detail.Location.Latitude}, {detail.Location.Longitude}";
        var evidenceSummary =
            $"Selfie: {detail?.HasSelfie}, Photos: {detail?.Photos.Count ?? 0}, Signature: {detail?.HasSignature}";

        var tenantId = CctvTenantHelper.ResolveTenantId(tenantContext, options.Value);
        var model = new
        {
            ScheduleNumber = schedule?.ScheduleNumber,
            SiteName = site?.Name,
            EngineerName = engineer?.Name,
            VisitDate = schedule?.ScheduledDate.ToString("yyyy-MM-dd"),
            ReportStatus = detail?.ReportStatus,
            GpsSummary = gpsSummary,
            EvidenceSummary = evidenceSummary,
            VisitRemarks = detail?.VisitRemarks,
            ApprovalStatus = "Approved"
        };

        var bytes = await pdfGenerationService.GenerateAsync("visit", model, cancellationToken);
        var fileName = $"visit-{schedule?.ScheduleNumber ?? notification.VisitId.ToString()}.pdf";
        var fileId = await fileStore.StorePdfAsync(
            tenantId,
            notification.ReviewedBy,
            fileName,
            bytes,
            cancellationToken);

        visit.AttachReportPdf(fileId, notification.ReviewedBy);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Generated visit report PDF for visit {VisitId} (file {FileId})",
            notification.VisitId,
            fileId);
    }
}
