using Ashraak.Cctv.Lead.Application.Mapping;
using LeadId = Ashraak.Cctv.Lead.Domain.Aggregates.Lead.LeadId;
using Ashraak.Cctv.Lead.Domain.Enums;
using Ashraak.Cctv.Lead.Domain.Repositories;
using Ashraak.SharedKernel.Contracts.Auth.Interfaces;
using Ashraak.SharedKernel.Contracts.CctvCrm;
using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Contracts.FeatureFlags.Interfaces;
using Ashraak.SharedKernel.Interfaces;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Lead.Application.Commands.CreateLeadActivity;

internal sealed class CreateLeadActivityCommandHandler(
    ILeadRepository repository,
    IAuthPermissionChecker permissionChecker,
    IFeatureFlagService featureFlags,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateLeadActivityCommand, Result<LeadActivityDto>>
{
    public async Task<Result<LeadActivityDto>> Handle(
        CreateLeadActivityCommand request,
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

        LeadActivityType activityType;
        try
        {
            activityType = LeadMapper.ParseActivityType(request.ActivityType);
        }
        catch (ArgumentException)
        {
            return Error.Validation("Leads.InvalidActivityType", "Invalid activity type.");
        }

        LeadStatus? fromStatus = null;
        LeadStatus? toStatus = null;

        if (!string.IsNullOrWhiteSpace(request.FromStatus))
        {
            try
            {
                fromStatus = LeadMapper.ParseStatus(request.FromStatus);
            }
            catch (ArgumentException)
            {
                return Error.Validation("Leads.InvalidFromStatus", "Invalid from status.");
            }
        }

        if (!string.IsNullOrWhiteSpace(request.ToStatus))
        {
            try
            {
                toStatus = LeadMapper.ParseStatus(request.ToStatus);
            }
            catch (ArgumentException)
            {
                return Error.Validation("Leads.InvalidToStatus", "Invalid to status.");
            }
        }

        try
        {
            var activity = lead.AddActivity(
                activityType,
                request.Description,
                fromStatus,
                toStatus,
                request.UserId);

            await unitOfWork.SaveChangesAsync(cancellationToken);
            return LeadMapper.ToActivity(activity);
        }
        catch (InvalidOperationException ex)
        {
            return Error.Conflict("Leads.InvalidState", ex.Message);
        }
    }
}
