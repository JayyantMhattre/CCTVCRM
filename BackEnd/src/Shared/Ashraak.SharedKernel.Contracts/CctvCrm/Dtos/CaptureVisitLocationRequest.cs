namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>POST /cctv/visits/{visitId}/location.</summary>
public sealed record CaptureVisitLocationRequest(
    decimal Latitude,
    decimal Longitude,
    DateTime CapturedAt);
