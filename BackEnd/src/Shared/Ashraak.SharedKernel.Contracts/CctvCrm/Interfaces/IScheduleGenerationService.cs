namespace Ashraak.SharedKernel.Contracts.CctvCrm.Interfaces;

/// <summary>Generates frequency-based schedules when an AMC term is activated.</summary>
public interface IScheduleGenerationService
{
    Task GenerateSchedulesForTermAsync(
        Guid contractId,
        Guid termId,
        Guid activatedBy,
        CancellationToken cancellationToken = default);
}
