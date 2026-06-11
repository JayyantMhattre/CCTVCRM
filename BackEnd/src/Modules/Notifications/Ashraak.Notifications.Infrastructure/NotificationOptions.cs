namespace Ashraak.Notifications.Infrastructure;

public sealed class NotificationOptions
{
    /// <summary>console | smtp | sendgrid | ses</summary>
    public string Provider { get; set; } = "console";

    public string TemplatesPath { get; set; } = "Templates";

    public SmtpOptions Smtp { get; set; } = new();
}

public sealed class SmtpOptions
{
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; } = 587;
    public bool UseSsl { get; set; } = true;
    public string FromAddress { get; set; } = "noreply@ashraak.local";
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
