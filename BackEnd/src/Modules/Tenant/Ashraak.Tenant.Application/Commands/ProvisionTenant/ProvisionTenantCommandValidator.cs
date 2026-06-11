using FluentValidation;

namespace Ashraak.Tenant.Application.Commands.ProvisionTenant;

/// <summary>
/// FluentValidation validator for <see cref="ProvisionTenantCommand"/>.
/// Validates the tenant name, slug format, and owner identifier before the handler runs.
/// </summary>
internal sealed class ProvisionTenantCommandValidator : AbstractValidator<ProvisionTenantCommand>
{
    /// <summary>Defines all validation rules for <see cref="ProvisionTenantCommand"/>.</summary>
    public ProvisionTenantCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Tenant name is required.")
            .MaximumLength(100).WithMessage("Tenant name cannot exceed 100 characters.");

        RuleFor(x => x.Slug)
            .NotEmpty().WithMessage("Tenant slug is required.")
            .MaximumLength(50).WithMessage("Tenant slug cannot exceed 50 characters.")
            .Matches(@"^[a-z0-9\-]+$").WithMessage("Tenant slug can only contain lowercase letters, numbers, and hyphens.");

        RuleFor(x => x.OwnerUserId)
            .NotEmpty().WithMessage("Owner user ID is required.");
    }
}
