using FluentValidation;

namespace Ashraak.Cctv.Service.Application.Commands.OfflineVisitSyncBatch;

internal sealed class OfflineVisitSyncBatchCommandValidator : AbstractValidator<OfflineVisitSyncBatchCommand>
{
    public OfflineVisitSyncBatchCommandValidator()
    {
        RuleFor(x => x.Items).NotNull();
        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.VisitId).NotEmpty();
            item.RuleFor(i => i.ClientCorrelationId).MaximumLength(100);
        });
    }
}
