namespace Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;

/// <summary>Add remark to a lead (POST /cctv/leads/{id}/remarks).</summary>
public sealed record CreateLeadRemarkRequest(string Text);
