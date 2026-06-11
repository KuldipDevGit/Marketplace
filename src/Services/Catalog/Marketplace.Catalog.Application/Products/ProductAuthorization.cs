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
}
