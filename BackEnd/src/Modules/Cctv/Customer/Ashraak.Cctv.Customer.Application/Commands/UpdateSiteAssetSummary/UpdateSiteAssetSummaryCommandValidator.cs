using FluentValidation;

namespace Ashraak.Cctv.Customer.Application.Commands.UpdateSiteAssetSummary;

internal sealed class UpdateSiteAssetSummaryCommandValidator : AbstractValidator<UpdateSiteAssetSummaryCommand>
{
    public UpdateSiteAssetSummaryCommandValidator()
    {
        RuleFor(x => x.SiteId).NotEmpty();
        RuleFor(x => x.RowVersion).GreaterThan((uint)0);
        RuleFor(x => x.CameraCount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.DvrCount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.NvrCount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.HardDiskCount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.SwitchCount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.RouterCount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.MonitorCount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Brand).MaximumLength(100);
        RuleFor(x => x.Model).MaximumLength(100);
        RuleFor(x => x.Remarks).MaximumLength(1000);
    }
}
