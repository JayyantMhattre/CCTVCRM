using Ashraak.Cctv.Lead.Application.Mapping;
using LeadId = Ashraak.Cctv.Lead.Domain.Aggregates.Lead.LeadId;
using Ashraak.Cctv.Lead.Domain.Repositories;
using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Contracts.CctvCrm.Interfaces;

namespace Ashraak.Cctv.Lead.Infrastructure.Services;

internal sealed class LeadLookupService(ILeadRepository repository) : ILeadLookupService
{
    public async Task<LeadSummaryDto?> GetByIdAsync(Guid leadId, CancellationToken cancellationToken = default)
    {
        var lead = await repository.GetByIdAsync(LeadId.From(leadId), cancellationToken);
        return lead is null ? null : LeadMapper.ToSummary(lead);
    }

    public async Task<LeadSummaryDto?> GetByNumberAsync(string leadNumber, CancellationToken cancellationToken = default)
    {
        var lead = await repository.GetByNumberAsync(leadNumber, cancellationToken);
        return lead is null ? null : LeadMapper.ToSummary(lead);
    }
}
