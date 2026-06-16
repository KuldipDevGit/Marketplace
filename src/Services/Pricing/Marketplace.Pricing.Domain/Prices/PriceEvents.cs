using Marketplace.SharedKernel;

namespace Marketplace.Pricing.Domain.Prices;

public sealed record ProductPriceSetDomainEvent(Guid PriceId, Guid ProductId, decimal Amount, string Currency) : IDomainEvent;

public sealed record ProductPriceChangedDomainEvent(Guid PriceId, Guid ProductId, decimal Amount, string Currency) : IDomainEvent;
