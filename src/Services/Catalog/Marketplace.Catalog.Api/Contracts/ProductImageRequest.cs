namespace Marketplace.Catalog.Api.Contracts;

public sealed record ProductImageRequest(string Url, string? AltText, int SortOrder = 0, bool IsPrimary = false);
