namespace Marketplace.Catalog.Api.Contracts;

public sealed class UpdateBrandRequest
{
    public required string Name { get; set; }
    public string? LogoUrl { get; set; }
    public required bool IsActive { get; set; }
}
