using Ashraak.Cctv.Lead.Application.Abstractions;
using Ashraak.Cctv.Lead.Application.Mapping;
using LeadAggregate = Ashraak.Cctv.Lead.Domain.Aggregates.Lead.Lead;
using LeadId = Ashraak.Cctv.Lead.Domain.Aggregates.Lead.LeadId;
using Ashraak.Cctv.Lead.Domain.Enums;
using Ashraak.Cctv.Lead.Domain.Repositories;
using Ashraak.SharedKernel.Contracts.CctvCrm;
using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Contracts.FeatureFlags.Interfaces;
using Ashraak.SharedKernel.Interfaces;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Lead.Application.Commands.CreateInquiry;

internal sealed class CreateInquiryCommandHandler(
    ILeadRepository repository,
    ILeadNumberGenerator numberGenerator,
    IFeatureFlagService featureFlags,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateInquiryCommand, Result<CreateInquiryResultDto>>
{
    public async Task<Result<CreateInquiryResultDto>> Handle(
        CreateInquiryCommand request,
        CancellationToken cancellationToken)
    {
        if (!await featureFlags.IsEnabledAsync(CctvFeatureFlags.LeadsEnabled, request.TenantId, cancellationToken))
            return Error.Forbidden("Leads.Disabled", "Lead management is not enabled for this tenant.");

        LeadSource source;
        try
        {
            source = LeadMapper.MapInquiryTypeToSource(request.InquiryType);
        }
        catch (ArgumentException)
        {
            return Error.Validation("Leads.InvalidInquiryType", "Invalid inquiry type.");
        }

        var leadNumber = await numberGenerator.GenerateNextAsync(cancellationToken);
        var lead = LeadAggregate.CreateFromInquiry(
            LeadId.New(),
            leadNumber,
            source,
            request.Name,
            request.Organization,
            request.Email,
            request.Phone,
            request.City,
            request.Address,
            request.RequirementSummary,
            request.UserId);

        repository.Add(lead);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateInquiryResultDto(lead.Id.Value, lead.LeadNumber);
    }
}
