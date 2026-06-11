using Marketplace.Catalog.Application.Common;
using Marketplace.Catalog.Application.Products;
using Marketplace.Catalog.Domain.Products;

namespace Marketplace.Catalog.Application.Abstractions;

public interface IProductRepository
{
    /// <summary>Loads a product with its images and attributes for mutation.</summary>
    Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<bool> SlugExistsAsync(string slug, Guid? excludingId = null, CancellationToken cancellationToken = default);

    Task AddAsync(Product product, CancellationToken cancellationToken = default);

    /// <summary>Applies the client-supplied concurrency token so EF detects lost updates (API-9, DB-8).</summary>
    void SetOriginalRowVersion(Product product, byte[] rowVersion);

    Task<ProductDto?> GetDtoByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<ProductDto?> GetDtoBySlugAsync(string slug, CancellationToken cancellationToken = default);

    Task<PagedResult<ProductSummaryDto>> ListAsync(ProductListFilter filter, CancellationToken cancellationToken = default);
}
