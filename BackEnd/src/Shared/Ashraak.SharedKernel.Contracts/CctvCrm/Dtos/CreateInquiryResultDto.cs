namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>Response from POST /cctv/inquiries.</summary>
public sealed record CreateInquiryResultDto(Guid LeadId, string LeadNumber);
