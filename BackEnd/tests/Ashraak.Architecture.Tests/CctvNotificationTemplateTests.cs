using Ashraak.Cctv.Integration.Application;
using FluentAssertions;
using Xunit;

namespace Ashraak.Architecture.Tests;

public sealed class CctvNotificationTemplateTests
{
    [Fact]
    public void CctvNotificationTemplateKeys_ShouldDefineAllFreezeSection17Events()
    {
        var keys = new[]
        {
            CctvNotificationTemplateKeys.LeadCreated,
            CctvNotificationTemplateKeys.LeadConverted,
            CctvNotificationTemplateKeys.CustomerWelcome,
            CctvNotificationTemplateKeys.AmcRenewalRequested,
            CctvNotificationTemplateKeys.AmcExpiryReminder,
            CctvNotificationTemplateKeys.VisitScheduled,
            CctvNotificationTemplateKeys.VisitCompleted,
            CctvNotificationTemplateKeys.VisitApproved,
            CctvNotificationTemplateKeys.TicketCreated,
            CctvNotificationTemplateKeys.TicketAssigned,
            CctvNotificationTemplateKeys.TicketClosed,
            CctvNotificationTemplateKeys.InvoiceGenerated
        };

        keys.Should().OnlyHaveUniqueItems();
        keys.Should().AllSatisfy(k => k.Should().StartWith("cctv/"));
    }
}
