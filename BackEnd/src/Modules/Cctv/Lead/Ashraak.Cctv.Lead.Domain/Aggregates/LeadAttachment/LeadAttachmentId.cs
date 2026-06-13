namespace Ashraak.Cctv.Lead.Domain.Aggregates.LeadAttachment;

public readonly record struct LeadAttachmentId(Guid Value)
{
    public static LeadAttachmentId New() => new(Guid.NewGuid());
    public static LeadAttachmentId From(Guid value) => new(value);
}
