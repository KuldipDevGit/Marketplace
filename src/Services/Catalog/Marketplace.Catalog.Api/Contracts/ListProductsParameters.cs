using Marketplace.Catalog.Domain.Products;
using Microsoft.AspNetCore.Mvc;

namespace Marketplace.Catalog.Api.Contracts;

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
