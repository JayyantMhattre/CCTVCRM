using Ashraak.Cctv.Lead.Domain.Aggregates.LeadActivity;
using LeadAggregate = Ashraak.Cctv.Lead.Domain.Aggregates.Lead.Lead;
using Ashraak.Cctv.Lead.Domain.Aggregates.LeadAttachment;
using Ashraak.Cctv.Lead.Domain.Aggregates.LeadRemark;
using Ashraak.Cctv.Lead.Domain.Enums;
using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Contracts.CctvCrm.Enums;

namespace Ashraak.Cctv.Lead.Application.Mapping;

internal static class LeadMapper
{
    public static LeadSummaryDto ToSummary(LeadAggregate lead) =>
        new(
            lead.Id.Value,
            lead.LeadNumber,
            ToStatusString(lead.Status),
            ToSourceString(lead.Source),
            lead.ContactName,
            lead.OrganizationName,
            lead.Email,
            lead.Phone,
            lead.City,
            lead.OwnerUserId,
            lead.CreatedAtUtc);

    public static LeadDetailDto ToDetail(LeadAggregate lead) =>
        new(
            lead.Id.Value,
            lead.LeadNumber,
            ToStatusString(lead.Status),
            ToSourceString(lead.Source),
            lead.ContactName,
            lead.OrganizationName,
            lead.Email,
            lead.Phone,
            lead.City,
            lead.Address,
            lead.RequirementSummary,
            lead.OwnerUserId,
            lead.ConvertedCustomerId,
            lead.ConvertedSiteId,
            lead.ConvertedContractId,
            lead.ConvertedAtUtc,
            lead.CreatedAtUtc,
            lead.CreatedBy,
            lead.UpdatedAtUtc,
            lead.UpdatedBy,
            lead.RowVersion,
            lead.Activities.Count,
            lead.Remarks.Count,
            lead.Attachments.Count(a => !a.IsDeleted));

    public static LeadActivityDto ToActivity(LeadActivity activity) =>
        new(
            activity.Id.Value,
            activity.LeadId.Value,
            ToActivityTypeString(activity.ActivityType),
            activity.FromStatus.HasValue ? ToStatusString(activity.FromStatus.Value) : null,
            activity.ToStatus.HasValue ? ToStatusString(activity.ToStatus.Value) : null,
            activity.Description,
            activity.OccurredAtUtc,
            activity.CreatedAtUtc,
            activity.CreatedBy);

    public static LeadRemarkDto ToRemark(LeadRemark remark) =>
        new(
            remark.Id.Value,
            remark.LeadId.Value,
            remark.Remark,
            remark.CreatedAtUtc,
            remark.CreatedBy);

    public static LeadAttachmentDto ToAttachment(LeadAttachment attachment) =>
        new(
            attachment.Id.Value,
            attachment.LeadId.Value,
            attachment.FileId,
            attachment.Title,
            attachment.CreatedAtUtc,
            attachment.CreatedBy);

    public static string ToStatusString(LeadStatus status) => status.ToString();

    public static string ToSourceString(LeadSource source) => source.ToString();

    public static string ToActivityTypeString(LeadActivityType activityType) => activityType.ToString();

    public static LeadStatus ParseStatus(string value)
    {
        if (!Enum.TryParse<LeadStatus>(value, ignoreCase: false, out var status))
            throw new ArgumentException($"Invalid lead status: {value}", nameof(value));

        return status;
    }

    public static LeadSource ParseSource(string value)
    {
        if (!Enum.TryParse<LeadSource>(value, ignoreCase: false, out var source))
            throw new ArgumentException($"Invalid lead source: {value}", nameof(value));

        return source;
    }

    public static LeadActivityType ParseActivityType(string value)
    {
        if (!Enum.TryParse<LeadActivityType>(value, ignoreCase: false, out var activityType))
            throw new ArgumentException($"Invalid activity type: {value}", nameof(value));

        return activityType;
    }

    public static LeadSource MapInquiryTypeToSource(string inquiryType) =>
        inquiryType switch
        {
            InquiryTypeContract.Contact => LeadSource.WebsiteContact,
            InquiryTypeContract.GetQuote => LeadSource.GetQuote,
            InquiryTypeContract.AmcInquiry => LeadSource.AmcInquiry,
            _ => throw new ArgumentException($"Invalid inquiry type: {inquiryType}", nameof(inquiryType))
        };

    public static bool TryParseStatus(string? value, out LeadStatus status)
    {
        status = default;
        return !string.IsNullOrWhiteSpace(value) && Enum.TryParse(value, ignoreCase: false, out status);
    }
}
