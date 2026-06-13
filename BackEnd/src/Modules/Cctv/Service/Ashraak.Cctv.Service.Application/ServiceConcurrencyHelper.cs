using Ashraak.SharedKernel.Results;
using ScheduleAggregate = Ashraak.Cctv.Service.Domain.Aggregates.Schedule.ServiceSchedule;
using VisitAggregate = Ashraak.Cctv.Service.Domain.Aggregates.Visit.ServiceVisit;

namespace Ashraak.Cctv.Service.Application;

internal static class ServiceConcurrencyHelper
{
    public static Error? EnsureRowVersion(ScheduleAggregate schedule, uint expectedRowVersion)
    {
        if (schedule.RowVersion != expectedRowVersion)
        {
            return Error.Conflict(
                "Schedules.ConcurrencyConflict",
                "The schedule was modified by another user. Refresh and try again.");
        }

        return null;
    }

    public static Error? EnsureRowVersion(VisitAggregate visit, uint expectedRowVersion)
    {
        if (visit.RowVersion != expectedRowVersion)
        {
            return Error.Conflict(
                "Visits.ConcurrencyConflict",
                "The visit was modified by another user. Refresh and try again.");
        }

        return null;
    }
}
