using Microsoft.AspNetCore.Mvc;

namespace Marketplace.Catalog.Api.Contracts;

/// <summary>Shared paging + sort query parameters for the list endpoints. Query-key names are pinned
/// so the OpenAPI parameters stay camelCase (matching openapi/catalog.v1.yaml and the generated client).</summary>
public class PagedQueryParameters
{
    [FromQuery(Name = "page")]
    public int Page { get; set; } = 1;

    [FromQuery(Name = "pageSize")]
    public int PageSize { get; set; } = 20;

    [FromQuery(Name = "sort")]
    public string? Sort { get; set; }
}
