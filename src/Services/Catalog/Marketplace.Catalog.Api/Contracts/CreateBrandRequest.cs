namespace Marketplace.Catalog.Api.Contracts;

public sealed record CreateBrandRequest(string Name, string? Slug, string? LogoUrl, bool IsActive = true);
