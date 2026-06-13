using Ashraak.Cctv.Amc.Application.Mapping;
using Ashraak.Cctv.Amc.Domain.Repositories;
using Ashraak.SharedKernel.Contracts.Auth.Interfaces;
using Ashraak.SharedKernel.Contracts.CctvCrm;
using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Contracts.CctvCrm.Interfaces;
using Ashraak.SharedKernel.Contracts.FeatureFlags.Interfaces;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Amc.Application.Queries.GetPortalAmcDocuments;

internal sealed class GetPortalAmcDocumentsQueryHandler(
    IAmcContractRepository contractRepository,
    ICustomerLookupService customerLookup,
    IAuthPermissionChecker permissionChecker,
    IFeatureFlagService featureFlags) : IRequestHandler<GetPortalAmcDocumentsQuery, Result<IReadOnlyList<AmcContractDocumentDto>>>
{
    public async Task<Result<IReadOnlyList<AmcContractDocumentDto>>> Handle(
        GetPortalAmcDocumentsQuery request,
        CancellationToken cancellationToken)
    {
        if (!await featureFlags.IsEnabledAsync(CctvFeatureFlags.AmcEnabled, request.TenantId, cancellationToken))
            return Error.Forbidden("Amc.Disabled", "AMC management is not enabled for this tenant.");

        var authError = await AmcContractAuthorization.EnsureCanAccessPortalAsync(
            permissionChecker, request.UserId, request.TenantId, cancellationToken);
        if (authError is not null)
            return authError;

        var customer = await customerLookup.GetCustomerForUserAsync(request.UserId, cancellationToken);
        if (customer is null)
            return Error.NotFound("Amc.PortalCustomerNotFound", "No customer profile linked to this user.");

        var contract = await contractRepository.GetByCustomerIdWithActiveTermAsync(customer.Id, cancellationToken);
        if (contract is null)
            return Error.NotFound("Amc.PortalNotFound", "No active AMC contract found for your account.");

        return contract.Documents.Select(AmcMapper.ToDocument).ToList();
    }
}
