namespace Marketplace.Catalog.Api.Contracts;

public sealed record AddProductImageRequest(string Url, string? AltText, int SortOrder = 0, bool IsPrimary = false);
