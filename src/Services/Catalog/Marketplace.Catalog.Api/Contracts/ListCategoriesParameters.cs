using Microsoft.AspNetCore.Mvc;

namespace Marketplace.Catalog.Api.Contracts;

public sealed class ListCategoriesParameters : PagedQueryParameters
{
    [FromQuery(Name = "parentId")]
    public Guid? ParentId { get; set; }

    [FromQuery(Name = "includeInactive")]
    public bool IncludeInactive { get; set; }
}
