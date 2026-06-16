using System.Linq.Expressions;
using Marketplace.Catalog.Application.Abstractions;
using Marketplace.Catalog.Application.Brands;
using Marketplace.Catalog.Application.Common;
using Marketplace.Catalog.Domain.Brands;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Catalog.Infrastructure.Persistence.Repositories;

internal sealed class BrandRepository(CatalogDbContext db) : IBrandRepository
{
    private static readonly Expression<Func<Brand, BrandDto>> ToDto = b => new BrandDto
    {
        Id = b.Id,
        Name = b.Name,
        Slug = b.Slug,
        LogoUrl = b.LogoUrl,
        IsActive = b.IsActive,
        CreatedAtUtc = b.CreatedAtUtc,
        UpdatedAtUtc = b.UpdatedAtUtc,
    };

    public Task<Brand?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        db.Brands.FirstOrDefaultAsync(b => b.Id == id, cancellationToken);

    public Task<bool> SlugExistsAsync(string slug, Guid? excludingId = null, CancellationToken cancellationToken = default) =>
        db.Brands.AnyAsync(b => b.Slug == slug && (excludingId == null || b.Id != excludingId), cancellationToken);

    public async Task AddAsync(Brand brand, CancellationToken cancellationToken = default) =>
        await db.Brands.AddAsync(brand, cancellationToken);

    public Task<BrandDto?> GetDtoByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        db.Brands.AsNoTracking().Where(b => b.Id == id).Select(ToDto).FirstOrDefaultAsync(cancellationToken);

    public async Task<PagedResult<BrandDto>> ListAsync(int page, int pageSize, string? sort, CancellationToken cancellationToken = default)
    {
        var query = db.Brands.AsNoTracking();

        var total = await query.LongCountAsync(cancellationToken);
        var items = await query
            .OrderBy(b => b.Name)
            .Skip((page - 1) * pageSize).Take(pageSize)
            .Select(ToDto)
            .ToListAsync(cancellationToken);

        return new PagedResult<BrandDto>(items, page, pageSize, total);
    }
}
