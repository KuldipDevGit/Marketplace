namespace Marketplace.Application.Abstractions;

/// <summary>
/// The authenticated caller, projected from the request's access token by the API (ADR-0006).
/// Used for RBAC and resource-ownership checks (SEC-6).
/// </summary>
public interface ICurrentUser
{
    bool IsAuthenticated { get; }

    Guid? UserId { get; }

    /// <summary>The seller this user acts as, when the seller role is present.</summary>
    Guid? SellerId { get; }

    bool IsAdmin { get; }

    bool IsSeller { get; }
}
