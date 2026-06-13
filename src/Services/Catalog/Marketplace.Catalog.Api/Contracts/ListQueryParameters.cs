using Marketplace.Catalog.Domain.Products;
using Microsoft.AspNetCore.Mvc;

namespace Marketplace.Catalog.Api.Contracts;

// Query-string parameter objects for the list endpoints, bound with [FromQuery]. The query-key names
// are pinned so the documented OpenAPI parameters stay camelCase — matching openapi/catalog.v1.yaml
// and the generated client. Paging + sort live in the shared base so they are not repeated per endpoint.

public class PagedQueryParameters
{
    [FromQuery(Name = "page")]
    public int Page { get; set; } = 1;

    [FromQuery(Name = "pageSize")]
    public int PageSize { get; set; } = 20;

    [FromQuery(Name = "sort")]
    public string? Sort { get; set; }
}

public sealed class ListCategoriesParameters : PagedQueryParameters
{
    [FromQuery(Name = "parentId")]
    public Guid? ParentId { get; set; }

    [FromQuery(Name = "includeInactive")]
    public bool IncludeInactive { get; set; }
}

public sealed class ListProductsParameters : PagedQueryParameters
{
    [FromQuery(Name = "categoryId")]
    public Guid? CategoryId { get; set; }

    [FromQuery(Name = "sellerId")]
    public Guid? SellerId { get; set; }

    [FromQuery(Name = "brandId")]
    public Guid? BrandId { get; set; }

    [FromQuery(Name = "status")]
    public ProductStatus? Status { get; set; }

    [FromQuery(Name = "q")]
    public string? Q { get; set; }
}
