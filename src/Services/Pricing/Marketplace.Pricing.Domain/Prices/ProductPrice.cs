using Marketplace.SharedKernel;

namespace Marketplace.Pricing.Domain.Prices;

/// <summary>
/// The list price for a single catalog product — the Pricing bounded context's aggregate (ADR-0013).
/// Keyed by its own id with a unique <see cref="ProductId"/>. Stock is owned by Inventory and product
/// detail by Catalog; this context references the product by id only (BE-9).
/// </summary>
public sealed class ProductPrice : AuditableAggregateRoot<Guid>
{
    private ProductPrice(Guid id) : base(id)
    {
    }

    public ProductPrice(Guid id, Guid productId, Money price) : base(id)
    {
        ProductId = productId;
        Price = price;
        RaiseDomainEvent(new ProductPriceSetDomainEvent(id, productId, price.Amount, price.Currency));
    }

    public Guid ProductId { get; private set; }
    public Money Price { get; private set; } = null!;

    /// <summary>Reprices the product, raising a change event only when the amount or currency actually differs.</summary>
    public void ChangePrice(Money price)
    {
        if (Price == price)
        {
            return;
        }

        Price = price;
        RaiseDomainEvent(new ProductPriceChangedDomainEvent(Id, ProductId, price.Amount, price.Currency));
    }
}
