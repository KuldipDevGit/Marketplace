namespace Marketplace.Catalog.Api.Contracts;

public sealed record UpdateBrandRequest(string Name, string? LogoUrl, bool IsActive);
