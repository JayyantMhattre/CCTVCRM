using Ashraak.Cctv.Lead.Application.Abstractions;
using Ashraak.Cctv.Lead.Application.Mapping;
using LeadAggregate = Ashraak.Cctv.Lead.Domain.Aggregates.Lead.Lead;
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

namespace Ashraak.Cctv.Lead.Application.Commands.CreateLead;

internal sealed class CreateLeadCommandHandler(
    ILeadRepository repository,
    ILeadNumberGenerator numberGenerator,
    IAuthPermissionChecker permissionChecker,
    IFeatureFlagService featureFlags,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateLeadCommand, Result<LeadDetailDto>>
{
    public async Task<Result<LeadDetailDto>> Handle(
        CreateLeadCommand request,
        CancellationToken cancellationToken)
    {
        if (!await featureFlags.IsEnabledAsync(CctvFeatureFlags.LeadsEnabled, request.TenantId, cancellationToken))
            return Error.Forbidden("Leads.Disabled", "Lead management is not enabled for this tenant.");

        var authError = await LeadAuthorization.EnsureCanManageAsync(
            permissionChecker, request.UserId, request.TenantId, cancellationToken);
        if (authError is not null)
            return authError;

        LeadSource source;
        try
        {
            source = LeadMapper.ParseSource(request.Source);
        }
        catch (ArgumentException)
        {
            return Error.Validation("Leads.InvalidSource", "Invalid lead source.");
        }

        var leadNumber = await numberGenerator.GenerateNextAsync(cancellationToken);
        var lead = LeadAggregate.CreateManual(
            LeadId.New(),
            leadNumber,
            source,
            request.ContactName,
            request.OrganizationName,
            request.Email,
            request.Phone,
            request.City,
            request.Address,
            request.RequirementSummary,
            request.OwnerUserId,
            request.UserId);

        repository.Add(lead);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return LeadMapper.ToDetail(lead);
    }
}
