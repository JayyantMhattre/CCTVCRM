using Ashraak.Auth.Domain.Aggregates.AuthUser;
using Ashraak.Auth.Domain.Aggregates.Invitation;
using Ashraak.Auth.Domain.Entities;
using Ashraak.Auth.Infrastructure.Persistence.Authorization;
using Ashraak.BuildingBlocks.Infrastructure.Outbox;
using Ashraak.SharedKernel.Interfaces;
using Ashraak.SharedKernel.Outbox;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Ashraak.Auth.Infrastructure.Persistence;

/// <summary>
/// EF Core DbContext for the Auth module.
/// Extends <see cref="IdentityDbContext{TUser,TRole,TKey}"/> so that ASP.NET Core Identity
/// and OpenIddict can manage their own tables within the <c>auth</c> PostgreSQL schema.
/// </summary>
/// <remarks>
/// <para>
/// All Auth module tables live in the <c>auth</c> schema, isolated from other modules.
/// This enables schema-level access control and makes module extraction to a separate
/// database straightforward (Phase 3 microservice extraction).
/// </para>
/// <para>
/// Implements <see cref="IUnitOfWork"/> so that command handlers can depend on the
/// abstraction rather than the concrete DbContext type.
/// </para>
/// </remarks>
public sealed class AuthDbContext : IdentityDbContext<IdentityUser<Guid>, IdentityRole<Guid>, Guid>, IUnitOfWork
{
    /// <summary>
    /// Initialises the context with EF Core options configured by <c>AuthModule.AddAuthModule</c>.
    /// </summary>
    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) { }

    /// <summary>Gets the <c>AuthUser</c> aggregate table used for credential management.</summary>
    public DbSet<AuthUser> AuthUsers => Set<AuthUser>();

    public DbSet<Invitation> Invitations => Set<Invitation>();

    public DbSet<UserSession> UserSessions => Set<UserSession>();

    /// <summary>Gets user-role assignments used by RBAC checks.</summary>
    internal DbSet<AuthRoleAssignment> RoleAssignments => Set<AuthRoleAssignment>();

    /// <summary>Gets role/user permission grants used by RBAC + ABAC checks.</summary>
    internal DbSet<AuthPermissionGrant> PermissionGrants => Set<AuthPermissionGrant>();

    /// <summary>
    /// Gets the outbox messages table used for reliable domain event delivery.
    /// Domain events raised by <see cref="AuthUser"/> are serialised here by
    /// <c>BaseDbContext.SerializeDomainEventsToOutbox</c> before being dispatched by the outbox processor.
    /// </summary>
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    /// <inheritdoc/>
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        OutboxDomainEventSerializer.SerializeTrackedDomainEvents(this, OutboxMessages);
        return await base.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.HasDefaultSchema("auth");
        builder.ApplyConfigurationsFromAssembly(typeof(AuthDbContext).Assembly);
        OutboxModelConfiguration.ConfigureOutboxMessages(builder);
        builder.UseOpenIddict();
    }
}
