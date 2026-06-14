namespace Marketplace.Catalog.Application.Products;

public sealed class ProductImageDto
{
    public Guid Id { get; set; }
    public required string Url { get; set; }
    public string? AltText { get; set; }
    public int SortOrder { get; set; }
    public bool IsPrimary { get; set; }
}
