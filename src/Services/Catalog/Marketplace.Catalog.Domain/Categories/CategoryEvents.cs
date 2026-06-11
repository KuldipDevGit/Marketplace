using Marketplace.SharedKernel;

namespace Marketplace.Catalog.Domain.Categories;

public sealed record CategoryCreatedDomainEvent(Guid CategoryId, string Name, string Slug, Guid? ParentCategoryId) : IDomainEvent;

public sealed record CategoryUpdatedDomainEvent(Guid CategoryId, string Name, string Slug) : IDomainEvent;
