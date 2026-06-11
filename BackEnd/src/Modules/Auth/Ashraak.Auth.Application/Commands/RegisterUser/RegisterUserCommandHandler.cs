using Ashraak.Auth.Application.Abstractions;
using Ashraak.Auth.Domain.Aggregates.AuthUser;
using Ashraak.Auth.Domain.Repositories;
using Ashraak.SharedKernel.Contracts.Tenant.Interfaces;
using Ashraak.SharedKernel.Interfaces;
using Ashraak.SharedKernel.Results;
using MediatR;

namespace Ashraak.Auth.Application.Commands.RegisterUser;

/// <summary>
/// Handles <see cref="RegisterUserCommand"/>.
/// Enforces uniqueness within a tenant, hashes the password, and persists
/// a new <c>AuthUser</c> aggregate via the repository + unit of work.
/// </summary>
/// <remarks>
/// This handler does not create the <c>UserProfile</c> in the Users module directly.
/// That is handled reactively: the <c>TenantProvisionedDomainEvent</c> (or a future
/// <c>UserRegisteredDomainEvent</c>) triggers the Users module's <c>CreateUserProfileCommandHandler</c>
/// via the outbox processor, preserving module isolation.
/// </remarks>
internal sealed class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Result<Guid>>
{
    private readonly IAuthUserRepository _authUserRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITenantService _tenantService;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initialises the handler with its dependencies via constructor injection.
    /// </summary>
    public RegisterUserCommandHandler(
        IAuthUserRepository authUserRepository,
        IPasswordHasher passwordHasher,
        ITenantService tenantService,
        IUnitOfWork unitOfWork)
    {
        _authUserRepository = authUserRepository;
        _passwordHasher = passwordHasher;
        _tenantService = tenantService;
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc/>
    public async Task<Result<Guid>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        if (!await _tenantService.IsActiveAsync(request.TenantId, cancellationToken))
            return Error.Forbidden("Auth.TenantInactive", "The tenant is not active.");

        if (await _authUserRepository.ExistsAsync(request.Email, request.TenantId, cancellationToken))
            return Error.Conflict("Auth.EmailTaken", "This email is already registered.");

        var userId = Guid.NewGuid();
        var passwordHash = _passwordHasher.Hash(request.Password);

        var user = AuthUser.Create(
            userId,
            request.TenantId,
            request.Email,
            passwordHash,
            request.DisplayName);
        _authUserRepository.Add(user);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return userId;
    }
}
