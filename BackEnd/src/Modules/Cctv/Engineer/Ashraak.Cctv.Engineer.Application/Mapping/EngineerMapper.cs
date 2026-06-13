using Ashraak.Cctv.Engineer.Domain.Enums;
using EngineerAggregate = Ashraak.Cctv.Engineer.Domain.Aggregates.Engineer.Engineer;
using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

namespace Ashraak.Cctv.Engineer.Application.Mapping;

internal static class EngineerMapper
{
    public static EngineerSummaryDto ToSummary(EngineerAggregate engineer) =>
        new(
            engineer.Id.Value,
            engineer.EngineerNumber,
            engineer.Name,
            engineer.Phone,
            ToStatusString(engineer.Status),
            engineer.PlatformUserId,
            engineer.CreatedAtUtc);

    public static EngineerDetailDto ToDetail(EngineerAggregate engineer) =>
        new(
            engineer.Id.Value,
            engineer.EngineerNumber,
            engineer.Name,
            engineer.Phone,
            ToStatusString(engineer.Status),
            engineer.PlatformUserId,
            engineer.CreatedAtUtc,
            engineer.CreatedBy,
            engineer.UpdatedAtUtc,
            engineer.UpdatedBy,
            engineer.RowVersion);

    public static string ToStatusString(EngineerStatus status) => status.ToString();

    public static EngineerStatus ParseStatus(string value)
    {
        if (!Enum.TryParse<EngineerStatus>(value, ignoreCase: false, out var status))
            throw new ArgumentException($"Invalid engineer status: {value}", nameof(value));

        return status;
    }

    public static bool TryParseStatus(string? value, out EngineerStatus status)
    {
        status = default;
        return !string.IsNullOrWhiteSpace(value) && Enum.TryParse(value, ignoreCase: false, out status);
    }
}
