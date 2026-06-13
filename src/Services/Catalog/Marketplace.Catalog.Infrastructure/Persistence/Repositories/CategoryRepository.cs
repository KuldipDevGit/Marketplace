using System.Linq.Expressions;
using Marketplace.Catalog.Application.Abstractions;
using Marketplace.Catalog.Application.Categories;
using Marketplace.Catalog.Application.Common;
using Marketplace.Catalog.Domain.Categories;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Catalog.Infrastructure.Persistence.Repositories;

internal sealed class CategoryRepository(CatalogDbContext db) : ICategoryRepository
{
    private static readonly Expression<Func<Category, CategoryDto>> ToDto = c => new CategoryDto(
        c.Id, c.ParentCategoryId, c.Name, c.Slug, c.Description, c.ImageUrl, c.SortOrder, c.IsActive, c.CreatedAtUtc, c.UpdatedAtUtc);

    public Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        db.Categories.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

    public Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default) =>
        db.Categories.AnyAsync(c => c.Id == id, cancellationToken);

    public Task<bool> SlugExistsAsync(string slug, Guid? excludingId = null, CancellationToken cancellationToken = default) =>
        db.Categories.AnyAsync(c => c.Slug == slug && (excludingId == null || c.Id != excludingId), cancellationToken);

    public async Task AddAsync(Category category, CancellationToken cancellationToken = default) =>
        await db.Categories.AddAsync(category, cancellationToken);

    public Task<CategoryDto?> GetDtoByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        db.Categories.AsNoTracking().Where(c => c.Id == id).Select(ToDto).FirstOrDefaultAsync(cancellationToken);

    public async Task<PagedResult<CategoryDto>> ListAsync(
        Guid? parentId,
        bool includeInactive,
        int page,
        int pageSize,
        string? sort,
        CancellationToken cancellationToken = default)
    {
        var query = db.Categories.AsNoTracking();

        if (parentId is not null)
        {
            query = query.Where(c => c.ParentCategoryId == parentId);
        }

        if (!includeInactive)
        {
            query = query.Where(c => c.IsActive);
        }

        var total = await query.LongCountAsync(cancellationToken);
        var items = await query
            .OrderBy(c => c.SortOrder).ThenBy(c => c.Name)
            .Skip((page - 1) * pageSize).Take(pageSize)
            .Select(ToDto)
            .ToListAsync(cancellationToken);

        return new PagedResult<CategoryDto>(items, page, pageSize, total);
    }
}
