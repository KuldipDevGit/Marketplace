using Marketplace.Catalog.Application.Brands;
using Marketplace.Catalog.Application.Common;
using Marketplace.Catalog.Domain.Brands;

namespace Marketplace.Catalog.Application.Abstractions;

public interface IBrandRepository
{
    Task<Brand?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<bool> SlugExistsAsync(string slug, Guid? excludingId = null, CancellationToken cancellationToken = default);

    Task AddAsync(Brand brand, CancellationToken cancellationToken = default);

    Task<BrandDto?> GetDtoByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<PagedResult<BrandDto>> ListAsync(int page, int pageSize, string? sort, CancellationToken cancellationToken = default);
}
