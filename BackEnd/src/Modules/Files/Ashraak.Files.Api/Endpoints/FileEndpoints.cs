using Ashraak.Files.Application.Commands.DeleteFile;
using Ashraak.Files.Application.Commands.UploadFile;
using Ashraak.Files.Application.Queries.DownloadFile;
using Ashraak.Files.Application.Queries.GetFileUrl;
using Ashraak.SharedKernel.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Ashraak.Files.Api.Endpoints;

public static class FileEndpoints
{
    public static IEndpointRouteBuilder MapFileEndpoints(this IEndpointRouteBuilder routeBuilder)
    {
        var group = routeBuilder.MapGroup("/files")
            .WithTags("Files")
            .RequireAuthorization()
            .DisableAntiforgery();

        group.MapPost("/", UploadFile)
            .WithName("UploadFile")
            .WithSummary("Upload a tenant-scoped file (multipart).");

        group.MapGet("/{fileId:guid}", DownloadFile)
            .WithName("DownloadFile")
            .WithSummary("Download a file (auth-gated stream).");

        group.MapGet("/{fileId:guid}/url", GetFileUrl)
            .WithName("GetFileUrl")
            .WithSummary("Returns authenticated API path for download.");

        group.MapDelete("/{fileId:guid}", DeleteFile)
            .WithName("DeleteFile")
            .WithSummary("Soft-delete a file and remove blob.");

        return routeBuilder;
    }

    private static async Task<IResult> UploadFile(
        HttpRequest request,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        if (!request.HasFormContentType)
            return Results.BadRequest("Expected multipart/form-data.");

        var form = await request.ReadFormAsync(cancellationToken);
        var file = form.Files.GetFile("file");
        if (file is null || file.Length == 0)
            return Results.BadRequest("Missing file field.");

        await using var stream = file.OpenReadStream();
        var result = await sender.Send(
            new UploadFileCommand(
                tenantContext.TenantId,
                currentUser.UserId,
                file.FileName,
                file.ContentType ?? "application/octet-stream",
                stream,
                file.Length),
            cancellationToken);

        return result.IsFailure
            ? Results.Problem(result.Error.Description, statusCode: StatusCodes.Status400BadRequest)
            : Results.Created($"/api/v1/files/{result.Value.Id}", result.Value);
    }

    private static async Task<IResult> DownloadFile(
        Guid fileId,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new DownloadFileQuery(fileId, tenantContext.TenantId, currentUser.UserId),
            cancellationToken);

        if (result.IsFailure)
            return Results.NotFound();

        return Results.File(result.Value.Content, result.Value.ContentType, result.Value.FileName);
    }

    private static async Task<IResult> GetFileUrl(
        Guid fileId,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetFileUrlQuery(fileId, tenantContext.TenantId), cancellationToken);
        return result.IsFailure ? Results.NotFound() : Results.Ok(new { Url = result.Value });
    }

    private static async Task<IResult> DeleteFile(
        Guid fileId,
        ICurrentUser currentUser,
        ITenantContext tenantContext,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new DeleteFileCommand(fileId, tenantContext.TenantId, currentUser.UserId),
            cancellationToken);

        return result.IsFailure ? Results.NotFound() : Results.NoContent();
    }
}
