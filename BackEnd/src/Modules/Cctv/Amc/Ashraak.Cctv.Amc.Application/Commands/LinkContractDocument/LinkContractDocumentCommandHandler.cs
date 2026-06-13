using Ashraak.Cctv.Amc.Application.Mapping;
using Ashraak.Cctv.Amc.Domain.Aggregates.Contract;
using Ashraak.Cctv.Amc.Domain.Repositories;
using Ashraak.SharedKernel.Contracts.Auth.Interfaces;
using Ashraak.SharedKernel.Contracts.CctvCrm;
using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Contracts.FeatureFlags.Interfaces;
using Ashraak.SharedKernel.Interfaces;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Amc.Application.Commands.LinkContractDocument;

internal sealed class LinkContractDocumentCommandHandler(
    IAmcContractRepository contractRepository,
    IAuthPermissionChecker permissionChecker,
    IFeatureFlagService featureFlags,
    IUnitOfWork unitOfWork) : IRequestHandler<LinkContractDocumentCommand, Result<AmcContractDocumentDto>>
{
    public async Task<Result<AmcContractDocumentDto>> Handle(
        LinkContractDocumentCommand request,
        CancellationToken cancellationToken)
    {
        if (!await featureFlags.IsEnabledAsync(CctvFeatureFlags.AmcEnabled, request.TenantId, cancellationToken))
            return Error.Forbidden("Amc.Disabled", "AMC management is not enabled for this tenant.");

        var authError = await AmcContractAuthorization.EnsureCanManageAsync(
            permissionChecker, request.UserId, request.TenantId, cancellationToken);
        if (authError is not null)
            return authError;

        var contract = await contractRepository.GetByIdAsync(AmcContractId.From(request.ContractId), cancellationToken);
        if (contract is null)
            return Error.NotFound("Amc.NotFound", "AMC contract not found.");

        AmcContractTermId? termId = request.TermId.HasValue
            ? AmcContractTermId.From(request.TermId.Value)
            : null;

        if (termId.HasValue)
            _ = contract.GetTerm(termId.Value);

        var document = contract.LinkDocument(
            AmcContractDocumentId.New(),
            request.FileId,
            AmcMapper.ParseDocumentType(request.DocumentType),
            request.Title,
            termId,
            request.UserId);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return AmcMapper.ToDocument(document);
    }
}
