using LeadId = Ashraak.Cctv.Lead.Domain.Aggregates.Lead.LeadId;
using Ashraak.Cctv.Lead.Domain.Aggregates.LeadAttachment;
using Ashraak.Cctv.Lead.Domain.Repositories;
using Ashraak.SharedKernel.Contracts.Auth.Interfaces;
using Ashraak.SharedKernel.Contracts.CctvCrm;
using Ashraak.SharedKernel.Contracts.FeatureFlags.Interfaces;
using Ashraak.SharedKernel.Interfaces;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Lead.Application.Commands.RemoveLeadAttachment;

internal sealed class RemoveLeadAttachmentCommandHandler(
    ILeadRepository repository,
    IAuthPermissionChecker permissionChecker,
    IFeatureFlagService featureFlags,
    IUnitOfWork unitOfWork) : IRequestHandler<RemoveLeadAttachmentCommand, Result>
{
    public async Task<Result> Handle(
        RemoveLeadAttachmentCommand request,
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

        try
        {
            lead.RemoveAttachment(LeadAttachmentId.From(request.AttachmentId), request.UserId);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
        catch (InvalidOperationException ex)
        {
            return Error.NotFound("Leads.AttachmentNotFound", ex.Message);
        }
    }
}
