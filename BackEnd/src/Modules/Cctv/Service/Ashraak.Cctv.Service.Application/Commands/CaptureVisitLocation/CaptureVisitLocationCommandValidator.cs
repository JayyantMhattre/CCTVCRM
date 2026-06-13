using FluentValidation;

namespace Ashraak.Cctv.Service.Application.Commands.CaptureVisitLocation;

internal sealed class CaptureVisitLocationCommandValidator : AbstractValidator<CaptureVisitLocationCommand>
{
    public CaptureVisitLocationCommandValidator()
    {
        RuleFor(x => x.VisitId).NotEmpty();
        RuleFor(x => x.Latitude).InclusiveBetween(-90m, 90m);
        RuleFor(x => x.Longitude).InclusiveBetween(-180m, 180m);
        RuleFor(x => x.CapturedAt).NotEmpty();
    }
}
