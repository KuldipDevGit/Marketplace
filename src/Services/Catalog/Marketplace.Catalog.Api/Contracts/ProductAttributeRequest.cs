namespace Marketplace.Catalog.Api.Contracts;

public sealed class ProductAttributeRequest
{
    public required string Name { get; set; }
    public required string Value { get; set; }
}
