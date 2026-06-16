namespace Marketplace.Catalog.Api.Contracts;

public sealed class CreateBrandRequest
{
    public required string Name { get; set; }
    public string? Slug { get; set; }
    public string? LogoUrl { get; set; }
    public bool IsActive { get; set; } = true;
}
