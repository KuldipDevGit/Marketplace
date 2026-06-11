using Marketplace.Catalog.Application.Categories;
using Marketplace.Catalog.Application.Common;
using Marketplace.Catalog.Domain.Categories;

namespace Marketplace.Catalog.Application.Abstractions;

public interface ICategoryRepository
{
    Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);

    Task<bool> SlugExistsAsync(string slug, Guid? excludingId = null, CancellationToken cancellationToken = default);

    Task AddAsync(Category category, CancellationToken cancellationToken = default);

    Task<CategoryDto?> GetDtoByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<PagedResult<CategoryDto>> ListAsync(
        Guid? parentId,
        bool includeInactive,
        int page,
        int pageSize,
        string? sort,
        CancellationToken cancellationToken = default);
}
