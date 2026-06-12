using System.Security.Claims;
using Marketplace.Catalog.Application.Abstractions;

namespace Marketplace.Catalog.Api.Infrastructure;

/// <summary>Projects the authenticated caller from the request's JWT claims (ADR-0006, SEC-6).</summary>
internal sealed class CurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    private ClaimsPrincipal? Principal => httpContextAccessor.HttpContext?.User;

    public bool IsAuthenticated => Principal?.Identity?.IsAuthenticated ?? false;

    public Guid? UserId =>
        Guid.TryParse(
            Principal?.FindFirstValue("oid")
            ?? Principal?.FindFirstValue("sub")
            ?? Principal?.FindFirstValue(ClaimTypes.NameIdentifier),
            out var id)
            ? id
            : null;

    public Guid? SellerId =>
        Guid.TryParse(Principal?.FindFirstValue("seller_id"), out var id) ? id : null;

    public bool IsAdmin => Principal?.IsInRole("Admin") ?? false;

    public bool IsSeller => Principal?.IsInRole("Seller") ?? false;
}
