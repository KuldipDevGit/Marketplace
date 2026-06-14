namespace Marketplace.Catalog.Api.Contracts;

public sealed class AddProductImageRequest
{
    public required string Url { get; set; }
    public string? AltText { get; set; }
    public int SortOrder { get; set; }
    public bool IsPrimary { get; set; }
}
