using Ashraak.SharedKernel.Domain.Events;

namespace Ashraak.Cctv.Engineer.Domain.Aggregates.Engineer.Events;

/// <summary>Raised when a new engineer record is created.</summary>
public sealed record EngineerCreatedDomainEvent(
    Guid EngineerId,
    string EngineerNumber,
    string Name,
    Guid CreatedBy) : DomainEvent;

/// <summary>Raised when engineer profile fields are updated.</summary>
public sealed record EngineerUpdatedDomainEvent(
    Guid EngineerId,
    string EngineerNumber,
    Guid UpdatedBy) : DomainEvent;

/// <summary>Raised when an engineer is deactivated.</summary>
public sealed record EngineerDeactivatedDomainEvent(
    Guid EngineerId,
    string EngineerNumber,
    Guid DeactivatedBy) : DomainEvent;
