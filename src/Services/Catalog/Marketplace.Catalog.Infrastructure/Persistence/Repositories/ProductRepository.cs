using System.Linq.Expressions;
using Marketplace.Catalog.Application.Abstractions;
using Marketplace.Catalog.Application.Common;
using Marketplace.Catalog.Application.Products;
using Marketplace.Catalog.Domain.Products;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Catalog.Infrastructure.Persistence.Repositories;

internal sealed class ProductRepository(CatalogDbContext db) : IProductRepository
{
    private static readonly Expression<Func<Product, ProductSummaryDto>> ToSummary = p => new ProductSummaryDto(
        p.Id,
        p.SellerId,
        p.Name,
        p.Slug,
        p.CategoryId,
        p.BrandId,
        p.Images.Where(i => i.IsPrimary).Select(i => i.Url).FirstOrDefault(),
        p.Status);

    public Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        db.Products.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public Task<bool> SlugExistsAsync(string slug, Guid? excludingId = null, CancellationToken cancellationToken = default) =>
        db.Products.AnyAsync(p => p.Slug == slug && (excludingId == null || p.Id != excludingId), cancellationToken);

    public async Task AddAsync(Product product, CancellationToken cancellationToken = default) =>
        await db.Products.AddAsync(product, cancellationToken);

    public void SetOriginalRowVersion(Product product, byte[] rowVersion) =>
        db.Entry(product).Property(p => p.RowVersion).OriginalValue = rowVersion;

    public async Task<ProductDto?> GetDtoByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var product = await db.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        return product is null ? null : ToDto(product);
    }

    public async Task<ProductDto?> GetDtoBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        var product = await db.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Slug == slug, cancellationToken);
        return product is null ? null : ToDto(product);
    }

    public async Task<PagedResult<ProductSummaryDto>> ListAsync(ProductListFilter filter, CancellationToken cancellationToken = default)
    {
        var query = db.Products.AsNoTracking();

        if (filter.ActiveOnly)
        {
            query = query.Where(p => p.Status == ProductStatus.Active);
        }
        else if (filter.Status is not null)
        {
            query = query.Where(p => p.Status == filter.Status);
        }

        if (filter.CategoryId is not null)
        {
            query = query.Where(p => p.CategoryId == filter.CategoryId);
        }

        if (filter.SellerId is not null)
        {
            query = query.Where(p => p.SellerId == filter.SellerId);
        }

        if (filter.BrandId is not null)
        {
            query = query.Where(p => p.BrandId == filter.BrandId);
        }

        if (!string.IsNullOrWhiteSpace(filter.Query))
        {
            var term = $"%{filter.Query.Trim()}%";
            query = query.Where(p => EF.Functions.Like(p.Name, term) || EF.Functions.Like(p.Description, term));
        }

        var total = await query.LongCountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(p => p.CreatedAtUtc)
            .Skip((filter.Page - 1) * filter.PageSize).Take(filter.PageSize)
            .Select(ToSummary)
            .ToListAsync(cancellationToken);

        return new PagedResult<ProductSummaryDto>(items, filter.Page, filter.PageSize, total);
    }

    private static ProductDto ToDto(Product p) => new(
        p.Id,
        p.SellerId,
        p.CategoryId,
        p.BrandId,
        p.Name,
        p.Slug,
        p.Description,
        p.Sku,
        p.Status,
        p.Attributes.Select(a => new ProductAttributeDto(a.Name, a.Value)).ToList(),
        p.Images.OrderBy(i => i.SortOrder).Select(i => new ProductImageDto(i.Id, i.Url, i.AltText, i.SortOrder, i.IsPrimary)).ToList(),
        p.CreatedAtUtc,
        p.UpdatedAtUtc,
        Convert.ToBase64String(p.RowVersion));
}
