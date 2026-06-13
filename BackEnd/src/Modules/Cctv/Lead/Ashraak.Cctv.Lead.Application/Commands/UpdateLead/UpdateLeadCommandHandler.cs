using Ashraak.Cctv.Lead.Application.Mapping;
using LeadId = Ashraak.Cctv.Lead.Domain.Aggregates.Lead.LeadId;
using Ashraak.Cctv.Lead.Domain.Repositories;
using Ashraak.SharedKernel.Contracts.Auth.Interfaces;
using Ashraak.SharedKernel.Contracts.CctvCrm;
using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Contracts.FeatureFlags.Interfaces;
using Ashraak.SharedKernel.Interfaces;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Lead.Application.Commands.UpdateLead;

internal sealed class UpdateLeadCommandHandler(
    ILeadRepository repository,
    IAuthPermissionChecker permissionChecker,
    IFeatureFlagService featureFlags,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateLeadCommand, Result<LeadDetailDto>>
{
    public async Task<Result<LeadDetailDto>> Handle(
        UpdateLeadCommand request,
        CancellationToken cancellationToken)
    {
        if (!await featureFlags.IsEnabledAsync(CctvFeatureFlags.LeadsEnabled, request.TenantId, cancellationToken))
            return Error.Forbidden("Leads.Disabled", "Lead management is not enabled for this tenant.");

        var authError = await LeadAuthorization.EnsureCanManageAsync(
            permissionChecker, request.UserId, request.TenantId, cancellationToken);
        if (authError is not null)
            return authError;

        var lead = await repository.GetByIdAsync(LeadId.From(request.LeadId), cancellationToken);
        if (lead is null)
            return Error.NotFound("Leads.NotFound", "Lead not found.");

        var concurrencyError = LeadConcurrencyHelper.EnsureRowVersion(lead, request.RowVersion);
        if (concurrencyError is not null)
            return concurrencyError;

        try
        {
            lead.UpdateDetails(
                request.ContactName,
                request.OrganizationName,
                request.Email,
                request.Phone,
                request.City,
                request.Address,
                request.RequirementSummary,
                request.OwnerUserId,
                request.UserId);
        }
        catch (InvalidOperationException ex)
        {
            return Error.Conflict("Leads.InvalidState", ex.Message);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return LeadMapper.ToDetail(lead);
    }
}
