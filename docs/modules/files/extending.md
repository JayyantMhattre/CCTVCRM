# Extending Files

## Use from another module

Inject `IFileStorage` (contract) or send MediatR commands via HTTP from client apps.

```csharp
await fileStorage.UploadAsync(new FileUploadRequest(
    tenantId, userId, "report.pdf", "application/pdf", stream), ct);
```

## Custom provider

1. Implement `IStorageProvider` in `Files.Infrastructure/Storage/`
2. Register in `FilesModule.cs` switch on `Storage:Provider`
3. Document config in [storage-providers.md](./storage-providers.md)

## AV integration

Replace `StubFileScanService` with real scanner implementing `IFileScanService`.
