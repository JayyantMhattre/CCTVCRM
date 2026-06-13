namespace Ashraak.Cctv.Service.Application.Abstractions;

public interface IServiceScheduleNumberGenerator
{
    Task<string> GenerateNextAsync(CancellationToken cancellationToken);
}
