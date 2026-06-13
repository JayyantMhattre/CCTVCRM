using Ashraak.Cctv.Service.Application.Abstractions;
using Ashraak.Cctv.Service.Application.Mapping;
using Ashraak.Cctv.Service.Domain.Aggregates.Schedule;
using Ashraak.Cctv.Service.Domain.Repositories;
using Ashraak.SharedKernel.Contracts.Auth.Interfaces;
using Ashraak.SharedKernel.Contracts.CctvCrm;
using Ashraak.SharedKernel.Contracts.CctvCrm.Dtos;
using Ashraak.SharedKernel.Contracts.CctvCrm.Interfaces;
using Ashraak.SharedKernel.Contracts.FeatureFlags.Interfaces;
using Ashraak.SharedKernel.Interfaces;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Cctv.Service.Application.Commands.CreateAdHocSchedule;

internal sealed class CreateAdHocScheduleCommandHandler(
    IServiceScheduleRepository scheduleRepository,
    IServiceVisitRepository visitRepository,
    IServiceScheduleNumberGenerator numberGenerator,
    IAmcContractLookupService amcContractLookup,
    IAuthPermissionChecker permissionChecker,
    IFeatureFlagService featureFlags,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateAdHocScheduleCommand, Result<ScheduleDetailDto>>
{
    public async Task<Result<ScheduleDetailDto>> Handle(
        CreateAdHocScheduleCommand request,
        CancellationToken cancellationToken)
    {
        if (!await featureFlags.IsEnabledAsync(CctvFeatureFlags.ServiceEnabled, request.TenantId, cancellationToken))
            return Error.Forbidden("Service.Disabled", "Service scheduling is not enabled for this tenant.");

        var authError = await ScheduleAuthorization.EnsureCanManageAsync(
            permissionChecker, request.UserId, request.TenantId, cancellationToken);
        if (authError is not null)
            return authError;

        var contract = await amcContractLookup.GetActiveContractForSiteAsync(request.SiteId, cancellationToken);
        if (contract is null)
            return Error.Validation("Schedules.NoActiveContract", "Site has no active AMC contract.");

        var term = await amcContractLookup.GetTermByIdAsync(contract.Id, request.ContractTermId, cancellationToken);
        if (term is null)
            return Error.NotFound("Schedules.TermNotFound", "AMC contract term not found.");

        if (!string.Equals(term.Status, "Active", StringComparison.OrdinalIgnoreCase))
            return Error.Validation("Schedules.TermNotActive", "Ad-hoc schedules require an active contract term.");

        if (contract.SiteId != request.SiteId)
            return Error.Validation("Schedules.SiteMismatch", "Site does not match the active contract.");

        try
        {
            var scheduleNumber = await numberGenerator.GenerateNextAsync(cancellationToken);
            var schedule = ServiceSchedule.CreateAdHoc(
                ServiceScheduleId.New(),
                scheduleNumber,
                request.ContractTermId,
                request.SiteId,
                request.ScheduledDate,
                request.UserId);

            scheduleRepository.Add(schedule);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            var visit = await visitRepository.GetByScheduleIdAsync(schedule.Id, cancellationToken);
            return ServiceMapper.ToScheduleDetail(schedule, visit);
        }
        catch (InvalidOperationException ex)
        {
            return Error.Validation("Schedules.CreateAdHocFailed", ex.Message);
        }
    }
}
