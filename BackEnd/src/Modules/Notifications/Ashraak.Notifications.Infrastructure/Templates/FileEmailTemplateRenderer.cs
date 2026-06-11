using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Ashraak.Notifications.Infrastructure.Templates;

internal sealed class FileEmailTemplateRenderer(
    IHostEnvironment hostEnvironment,
    IOptions<NotificationOptions> options)
{
    public async Task<(string Subject, string Body)> RenderAsync(
        string template,
        IReadOnlyDictionary<string, string> data,
        CancellationToken cancellationToken)
    {
        var path = Path.Combine(
            hostEnvironment.ContentRootPath,
            options.Value.TemplatesPath,
            $"{template}.txt");

        if (!File.Exists(path))
            throw new FileNotFoundException($"Email template not found: {template}", path);

        var content = await File.ReadAllTextAsync(path, cancellationToken);
        foreach (var (key, value) in data)
            content = content.Replace($"{{{{{key}}}}}", value, StringComparison.OrdinalIgnoreCase);

        var lines = content.Split('\n', 2, StringSplitOptions.TrimEntries);
        var subject = lines[0].StartsWith("Subject:", StringComparison.OrdinalIgnoreCase)
            ? lines[0]["Subject:".Length..].Trim()
            : $"Ashraak — {template}";
        var body = lines.Length > 1 ? lines[1] : content;

        return (subject, body);
    }
}
