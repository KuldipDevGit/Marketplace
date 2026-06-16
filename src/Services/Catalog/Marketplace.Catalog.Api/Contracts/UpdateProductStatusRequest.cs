using Marketplace.Catalog.Domain.Products;

namespace Marketplace.Catalog.Api.Contracts;

public sealed class UpdateProductStatusRequest
{
    public required ProductStatus Status { get; set; }
    public required string RowVersion { get; set; }
}
