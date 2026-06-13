using System.Text.Json;
using Ashraak.Cctv.Amc.Domain.Aggregates.Contract;
using Ashraak.Cctv.Amc.Domain.Aggregates.Plan;
using Ashraak.Cctv.Amc.Domain.Enums;
using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Contracts.CctvCrm.Enums;
using ContractAggregate = Ashraak.Cctv.Amc.Domain.Aggregates.Contract.AmcContract;
using PlanAggregate = Ashraak.Cctv.Amc.Domain.Aggregates.Plan.AmcPlan;

namespace Ashraak.Cctv.Amc.Application.Mapping;

internal static class AmcMapper
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public static AmcPlanSummaryDto ToPlanSummary(PlanAggregate plan) =>
        new(
            plan.Id.Value,
            plan.PlanCode,
            plan.Name,
            plan.Description,
            ToPlanStatus(plan.Status),
            plan.Versions.Count(v => v.Status == PlanVersionStatus.Published),
            plan.CreatedAtUtc);

    public static AmcPlanDetailDto ToPlanDetail(PlanAggregate plan) =>
        new(
            plan.Id.Value,
            plan.PlanCode,
            plan.Name,
            plan.Description,
            ToPlanStatus(plan.Status),
            plan.CreatedAtUtc,
            plan.CreatedBy,
            plan.UpdatedAtUtc,
            plan.UpdatedBy,
            plan.RowVersion,
            plan.Versions
                .OrderByDescending(v => v.VersionNo)
                .Select(ToPlanVersionSummary)
                .ToList());

    public static AmcPlanVersionSummaryDto ToPlanVersionSummary(AmcPlanVersion version) =>
        new(
            version.Id.Value,
            version.VersionNo,
            version.Price,
            version.VisitFrequencyPerYear,
            version.EffectiveFrom,
            ToPlanVersionStatus(version.Status),
            version.CreatedAtUtc);

    public static AmcPlanVersionDetailDto ToPlanVersionDetail(PlanAggregate plan, AmcPlanVersion version) =>
        new(
            version.Id.Value,
            plan.Id.Value,
            plan.PlanCode,
            plan.Name,
            version.VersionNo,
            version.Price,
            version.VisitFrequencyPerYear,
            ParseServices(version.IncludedServicesJson),
            version.SlaTerms,
            version.EffectiveFrom,
            ToPlanVersionStatus(version.Status),
            version.IsReferenced,
            version.CreatedAtUtc,
            version.CreatedBy);

    public static AmcContractSummaryDto ToContractSummary(
        ContractAggregate contract,
        string? planCode = null)
    {
        var activeTerm = contract.GetActiveTerm();
        return new AmcContractSummaryDto(
            contract.Id.Value,
            contract.ContractNumber,
            contract.SiteId,
            contract.CustomerId,
            ToContractStatus(contract.Status),
            activeTerm?.Id.Value,
            activeTerm?.EndDate,
            planCode,
            contract.CreatedAtUtc);
    }

    public static AmcContractDetailDto ToContractDetail(
        ContractAggregate contract,
        IReadOnlyDictionary<Guid, (string PlanCode, int VersionNo)> planVersionMeta)
    {
        var terms = contract.Terms
            .OrderByDescending(t => t.TermNo)
            .Select(t =>
            {
                planVersionMeta.TryGetValue(t.PlanVersionId.Value, out var meta);
                return ToTermSummary(t, meta.PlanCode, meta.VersionNo);
            })
            .ToList();

        return new AmcContractDetailDto(
            contract.Id.Value,
            contract.ContractNumber,
            contract.SiteId,
            contract.CustomerId,
            contract.SourceLeadId,
            ToContractStatus(contract.Status),
            contract.CreatedAtUtc,
            contract.CreatedBy,
            contract.UpdatedAtUtc,
            contract.UpdatedBy,
            contract.RowVersion,
            terms);
    }

    public static AmcContractTermSummaryDto ToTermSummary(
        AmcContractTerm term,
        string planCode,
        int planVersionNo) =>
        new(
            term.Id.Value,
            term.TermNo,
            term.PlanVersionId.Value,
            planCode,
            planVersionNo,
            term.StartDate,
            term.EndDate,
            term.AgreedPrice,
            ToTermStatus(term.Status),
            ToTermOrigin(term.Origin),
            term.RenewalRequestedByCustomer,
            term.RenewalRequestedAtUtc,
            term.RowVersion);

    public static AmcContractTermDetailDto ToTermDetail(
        AmcContractTerm term,
        PlanAggregate plan,
        AmcPlanVersion version) =>
        new(
            term.Id.Value,
            term.ContractId.Value,
            term.TermNo,
            version.Id.Value,
            plan.PlanCode,
            version.VersionNo,
            version.Price,
            version.VisitFrequencyPerYear,
            ParseServices(version.IncludedServicesJson),
            version.SlaTerms,
            term.StartDate,
            term.EndDate,
            term.AgreedPrice,
            ToTermStatus(term.Status),
            ToTermOrigin(term.Origin),
            term.RenewalRequestedByCustomer,
            term.RenewalRequestedAtUtc,
            term.RowVersion);

    public static AmcContractDocumentDto ToDocument(AmcContractDocument document) =>
        new(
            document.Id.Value,
            document.ContractId.Value,
            document.TermId?.Value,
            document.FileId,
            ToDocumentType(document.DocumentType),
            document.Title,
            document.CreatedAtUtc,
            document.CreatedBy);

    public static AmcRenewalRequestSummaryDto ToRenewalRequest(ContractAggregate contract, AmcContractTerm term) =>
        new(
            contract.Id.Value,
            contract.ContractNumber,
            contract.SiteId,
            contract.CustomerId,
            term.Id.Value,
            term.TermNo,
            term.EndDate,
            term.RenewalRequestedAtUtc!.Value);

    public static PortalAmcDto ToPortalAmc(ContractAggregate contract, PlanAggregate plan, AmcPlanVersion version, AmcContractTerm term) =>
        new(
            contract.Id.Value,
            contract.ContractNumber,
            contract.SiteId,
            term.Id.Value,
            term.TermNo,
            plan.PlanCode,
            plan.Name,
            term.AgreedPrice,
            version.VisitFrequencyPerYear,
            ParseServices(version.IncludedServicesJson),
            version.SlaTerms,
            term.StartDate,
            term.EndDate,
            ToTermStatus(term.Status));

    public static string SerializeServices(IReadOnlyList<string> services) =>
        JsonSerializer.Serialize(services, JsonOptions);

    public static IReadOnlyList<string> ParseServices(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return [];

        try
        {
            return JsonSerializer.Deserialize<List<string>>(json, JsonOptions) ?? [];
        }
        catch (JsonException)
        {
            return [json];
        }
    }

    public static PlanStatus? ParsePlanStatus(string? status) =>
        string.IsNullOrWhiteSpace(status) ? null : Enum.Parse<PlanStatus>(status, true);

    public static ContractStatus? ParseContractStatus(string? status) =>
        string.IsNullOrWhiteSpace(status) ? null : Enum.Parse<ContractStatus>(status, true);

    public static TermOrigin ParseTermOrigin(string termType) =>
        termType.Equals(TermOriginContract.Renewal, StringComparison.OrdinalIgnoreCase)
            ? TermOrigin.Renewal
            : TermOrigin.New;

    public static ContractDocumentType ParseDocumentType(string documentType) =>
        documentType switch
        {
            ContractDocumentTypeContract.SignedCopy => ContractDocumentType.SignedCopy,
            ContractDocumentTypeContract.Other => ContractDocumentType.Other,
            _ => ContractDocumentType.ContractPdf
        };

    private static string ToPlanStatus(PlanStatus status) => status.ToString();
    private static string ToPlanVersionStatus(PlanVersionStatus status) => status.ToString();
    private static string ToContractStatus(ContractStatus status) => status.ToString();
    private static string ToTermStatus(TermStatus status) => status.ToString();
    private static string ToTermOrigin(TermOrigin origin) => origin.ToString();
    private static string ToDocumentType(ContractDocumentType type) => type.ToString();
}
