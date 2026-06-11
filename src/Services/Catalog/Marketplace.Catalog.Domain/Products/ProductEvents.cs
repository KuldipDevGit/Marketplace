using Marketplace.SharedKernel;

namespace Marketplace.Catalog.Domain.Products;

public sealed record ProductCreatedDomainEvent(Guid ProductId, Guid SellerId, Guid CategoryId, string Name, string Slug, string Sku) : IDomainEvent;

public sealed record ProductUpdatedDomainEvent(Guid ProductId, Guid SellerId, Guid CategoryId, string Name, string Sku) : IDomainEvent;

public sealed record ProductStatusChangedDomainEvent(Guid ProductId, Guid SellerId, ProductStatus Status) : IDomainEvent;

public sealed record ProductDeletedDomainEvent(Guid ProductId, Guid SellerId) : IDomainEvent;
