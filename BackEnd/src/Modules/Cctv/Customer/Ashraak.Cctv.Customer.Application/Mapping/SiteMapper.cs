using Ashraak.Cctv.Customer.Domain.Aggregates.Site;
using Ashraak.Cctv.Customer.Domain.Enums;
using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using SiteAggregate = Ashraak.Cctv.Customer.Domain.Aggregates.Site.Site;

namespace Ashraak.Cctv.Customer.Application.Mapping;

internal static class SiteMapper
{
    public static SiteSummaryDto ToSummary(SiteAggregate site) =>
        new(
            site.Id.Value,
            site.SiteNumber,
            site.CustomerId.Value,
            site.Name,
            site.Address,
            site.City,
            ToStatusString(site.Status),
            site.CreatedAtUtc);

    public static SiteDetailDto ToDetail(SiteAggregate site) =>
        new(
            site.Id.Value,
            site.SiteNumber,
            site.CustomerId.Value,
            site.Name,
            site.Address,
            site.City,
            ToStatusString(site.Status),
            site.CreatedAtUtc,
            site.CreatedBy,
            site.UpdatedAtUtc,
            site.UpdatedBy,
            site.RowVersion);

    public static SiteContactDto ToContact(SiteContact contact) =>
        new(
            contact.Id.Value,
            contact.ContactSlot,
            contact.Name,
            contact.Designation,
            contact.Phone,
            contact.Email,
            contact.IsPrimary);

    public static SiteDocumentDto ToDocument(SiteDocument document) =>
        new(
            document.Id.Value,
            document.FileId,
            ToDocumentTypeString(document.DocumentType),
            document.Title,
            document.CreatedAtUtc,
            document.CreatedBy);

    public static SiteAssetSummaryDto ToAssetSummary(SiteAssetSummary summary) =>
        new(
            summary.Id.Value,
            summary.CameraCount,
            summary.DvrCount,
            summary.NvrCount,
            summary.HardDiskCount,
            summary.SwitchCount,
            summary.RouterCount,
            summary.MonitorCount,
            summary.Brand,
            summary.Model,
            summary.Remarks,
            summary.RowVersion);

    public static IReadOnlyList<SiteContactInput> ToContactInputs(
        IReadOnlyList<SiteContactInputDto> contacts) =>
        contacts.Select(c => new SiteContactInput(
            c.Name,
            c.Designation,
            c.Phone,
            c.Email,
            c.IsPrimary)).ToList();

    public static string ToStatusString(SiteStatus status) => status.ToString();

    public static SiteStatus ParseStatus(string value)
    {
        if (!Enum.TryParse<SiteStatus>(value, ignoreCase: false, out var status))
            throw new ArgumentException($"Invalid site status: {value}", nameof(value));

        return status;
    }

    public static bool TryParseStatus(string? value, out SiteStatus status)
    {
        status = default;
        return !string.IsNullOrWhiteSpace(value) && Enum.TryParse(value, ignoreCase: false, out status);
    }

    public static SiteDocumentType ParseDocumentType(string value)
    {
        if (!Enum.TryParse<SiteDocumentType>(value, ignoreCase: false, out var type))
            throw new ArgumentException($"Invalid document type: {value}", nameof(value));

        return type;
    }

    public static string ToDocumentTypeString(SiteDocumentType type) => type.ToString();
}
