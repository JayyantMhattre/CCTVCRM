using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Results;
using LeadAggregate = Ashraak.Cctv.Lead.Domain.Aggregates.Lead.Lead;

namespace Ashraak.Cctv.Lead.Application.Abstractions;

/// <summary>Orchestrates cross-module provisioning when a lead is converted.</summary>
public interface ILeadConversionOrchestrator
{
    Task<Result<LeadConversionResultDto>> ConvertAsync(
        LeadAggregate lead,
        ConvertLeadRequest request,
        CancellationToken cancellationToken);
}
