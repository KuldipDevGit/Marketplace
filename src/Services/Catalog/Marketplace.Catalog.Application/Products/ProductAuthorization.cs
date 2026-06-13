using Marketplace.Catalog.Application.Abstractions;
using Marketplace.Catalog.Application.Common;
using Marketplace.Catalog.Domain.Products;

namespace Marketplace.Catalog.Application.Products;

/// <summary>Resource-ownership checks for products: a seller may act only on their own (SEC-6).</summary>
internal static class ProductAuthorization
{
    public static void EnsureCanModify(ICurrentUser user, Product product)
    {
        if (user.IsAdmin)
        {
            return;
        }

        if (user.SellerId is { } sellerId && product.SellerId == sellerId)
        {
            return;
        }

        throw new ForbiddenException("You do not have permission to modify this product.");
    }

    /// <summary>
    /// Read visibility (SEC-6): everyone sees Active products; admins see any status; a seller sees
    /// their own listings in any status. Mirrors the list-query rule so detail and list stay consistent.
    /// </summary>
    public static bool CanView(ICurrentUser user, ProductStatus status, Guid sellerId)
    {
        if (status == ProductStatus.Active || user.IsAdmin)
        {
            return true;
        }

        return user.SellerId is { } sellerId2 && sellerId == sellerId2;
    }
}
