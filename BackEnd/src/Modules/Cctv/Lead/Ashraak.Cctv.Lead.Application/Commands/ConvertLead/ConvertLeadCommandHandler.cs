using Ashraak.Cctv.Lead.Application.Abstractions;
using LeadId = Ashraak.Cctv.Lead.Domain.Aggregates.Lead.LeadId;
using Ashraak.Cctv.Lead.Domain.Repositories;
using Ashraak.SharedKernel.Contracts.Auth.Interfaces;
using Ashraak.SharedKernel.Contracts.CctvCrm;
using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Contracts.FeatureFlags.Interfaces;
using Ashraak.SharedKernel.Interfaces;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Lead.Application.Commands.ConvertLead;

internal sealed class ConvertLeadCommandHandler(
    ILeadRepository repository,
    ILeadConversionOrchestrator conversionOrchestrator,
    IAuthPermissionChecker permissionChecker,
    IFeatureFlagService featureFlags,
    IUnitOfWork unitOfWork) : IRequestHandler<ConvertLeadCommand, Result<LeadConversionResultDto>>
{
    public async Task<Result<LeadConversionResultDto>> Handle(
        ConvertLeadCommand request,
        CancellationToken cancellationToken)
    {
        if (!await featureFlags.IsEnabledAsync(CctvFeatureFlags.LeadsEnabled, request.TenantId, cancellationToken))
            return Error.Forbidden("Leads.Disabled", "Lead management is not enabled for this tenant.");

        var authError = await LeadAuthorization.EnsureCanConvertAsync(
            permissionChecker, request.UserId, request.TenantId, cancellationToken);
        if (authError is not null)
            return authError;

        var lead = await repository.GetByIdAsync(LeadId.From(request.LeadId), cancellationToken);
        if (lead is null)
            return Error.NotFound("Leads.NotFound", "Lead not found.");

        var concurrencyError = LeadConcurrencyHelper.EnsureRowVersion(lead, request.RowVersion);
        if (concurrencyError is not null)
            return concurrencyError;

        var convertRequest = new ConvertLeadRequest(
            request.PlanVersionId,
            request.SiteName,
            request.SiteAddress,
            request.InitialTermStartDate,
            request.InitialTermEndDate,
            request.RowVersion);

        var conversionResult = await conversionOrchestrator.ConvertAsync(lead, convertRequest, cancellationToken);
        if (conversionResult.IsFailure)
            return conversionResult.Error;

        try
        {
            lead.Convert(
                conversionResult.Value.CustomerId,
                conversionResult.Value.SiteId,
                conversionResult.Value.ContractId,
                request.UserId);
        }
        catch (InvalidOperationException ex)
        {
            return Error.Conflict("Leads.InvalidState", ex.Message);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return conversionResult.Value;
    }
}
