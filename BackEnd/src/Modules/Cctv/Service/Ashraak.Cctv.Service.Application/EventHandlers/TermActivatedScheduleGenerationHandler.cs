using Ashraak.Cctv.Amc.Domain.Aggregates.Contract.Events;
using Ashraak.SharedKernel.Contracts.CctvCrm;
using Ashraak.SharedKernel.Contracts.CctvCrm.Interfaces;
using Ashraak.SharedKernel.Contracts.FeatureFlags.Interfaces;
using MediatR;

namespace Ashraak.Cctv.Service.Application.EventHandlers;

internal sealed class TermActivatedScheduleGenerationHandler(
    IScheduleGenerationService scheduleGeneration,
    IFeatureFlagService featureFlags) : INotificationHandler<TermActivatedDomainEvent>
{
    public async Task Handle(TermActivatedDomainEvent notification, CancellationToken cancellationToken)
    {
        if (!await featureFlags.IsEnabledAsync(CctvFeatureFlags.ServiceEnabled, cancellationToken: cancellationToken))
            return;

        await scheduleGeneration.GenerateSchedulesForTermAsync(
            notification.ContractId,
            notification.TermId,
            notification.ActivatedBy,
            cancellationToken);
    }
}
