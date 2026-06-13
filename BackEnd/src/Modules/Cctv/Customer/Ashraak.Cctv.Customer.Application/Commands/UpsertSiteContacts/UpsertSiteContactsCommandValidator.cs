using FluentValidation;

namespace Ashraak.Cctv.Customer.Application.Commands.UpsertSiteContacts;

internal sealed class UpsertSiteContactsCommandValidator : AbstractValidator<UpsertSiteContactsCommand>
{
    public UpsertSiteContactsCommandValidator()
    {
        RuleFor(x => x.SiteId).NotEmpty();
        RuleFor(x => x.RowVersion).GreaterThan((uint)0);
        RuleFor(x => x.Contacts).Must(c => c.Count <= 3)
            .WithMessage("A site can have at most 3 contacts.");

        RuleForEach(x => x.Contacts).ChildRules(contact =>
        {
            contact.RuleFor(c => c.Name).NotEmpty().MaximumLength(200);
            contact.RuleFor(c => c.Designation).MaximumLength(100);
            contact.RuleFor(c => c.Phone).NotEmpty().MaximumLength(32);
            contact.RuleFor(c => c.Email).EmailAddress().MaximumLength(320).When(c => !string.IsNullOrWhiteSpace(c.Email));
        });

        RuleFor(x => x.Contacts)
            .Must(contacts => contacts.Count == 0 || contacts.Count(c => c.IsPrimary) == 1)
            .WithMessage("Exactly one contact must be primary when contacts are specified.");
    }
}
