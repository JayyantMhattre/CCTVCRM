using Ashraak.Cctv.Engineer.Domain.Aggregates.Engineer.Events;
using Ashraak.Cctv.Engineer.Domain.Enums;
using Ashraak.SharedKernel.Domain.Primitives;

namespace Ashraak.Cctv.Engineer.Domain.Aggregates.Engineer;

/// <summary>Engineer aggregate root (schema <c>cctv_engineer.engineers</c>).</summary>
public sealed class Engineer : AggregateRoot<EngineerId>
{
    private Engineer(EngineerId id) : base(id) { }

    public string EngineerNumber { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string Phone { get; private set; } = string.Empty;
    public EngineerStatus Status { get; private set; }
    public Guid? PlatformUserId { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public Guid CreatedBy { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }
    public Guid? UpdatedBy { get; private set; }
    public uint RowVersion { get; private set; }

    public bool IsActive => Status == EngineerStatus.Active;

    public static Engineer Create(
        EngineerId id,
        string engineerNumber,
        string name,
        string phone,
        Guid? platformUserId,
        Guid createdBy)
    {
        if (string.IsNullOrWhiteSpace(engineerNumber))
            throw new ArgumentException("Engineer number is required.", nameof(engineerNumber));

        var engineer = new Engineer(id)
        {
            EngineerNumber = engineerNumber.Trim(),
            Name = name.Trim(),
            Phone = phone.Trim(),
            Status = EngineerStatus.Active,
            PlatformUserId = platformUserId,
            CreatedAtUtc = DateTime.UtcNow,
            CreatedBy = createdBy,
            RowVersion = 1
        };

        engineer.RaiseDomainEvent(new EngineerCreatedDomainEvent(
            id.Value, engineer.EngineerNumber, engineer.Name, createdBy));

        return engineer;
    }

    public void Update(string name, string phone, Guid? platformUserId, Guid updatedBy)
    {
        Name = name.Trim();
        Phone = phone.Trim();
        PlatformUserId = platformUserId;
        Touch(updatedBy);
        RaiseDomainEvent(new EngineerUpdatedDomainEvent(Id.Value, EngineerNumber, updatedBy));
    }

    public void ChangeStatus(EngineerStatus status, Guid updatedBy)
    {
        if (Status == status)
            return;

        if (status == EngineerStatus.Inactive)
        {
            Status = EngineerStatus.Inactive;
            Touch(updatedBy);
            RaiseDomainEvent(new EngineerDeactivatedDomainEvent(Id.Value, EngineerNumber, updatedBy));
            return;
        }

        if (Status == EngineerStatus.Inactive && status == EngineerStatus.Active)
        {
            Status = EngineerStatus.Active;
            Touch(updatedBy);
            RaiseDomainEvent(new EngineerUpdatedDomainEvent(Id.Value, EngineerNumber, updatedBy));
        }
    }

    private void Touch(Guid updatedBy)
    {
        UpdatedAtUtc = DateTime.UtcNow;
        UpdatedBy = updatedBy;
        RowVersion++;
    }
}
