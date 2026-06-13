using Marketplace.Catalog.Domain.Products;

namespace Marketplace.Catalog.Api.Contracts;

public sealed record UpdateProductStatusRequest(ProductStatus Status, string RowVersion);
