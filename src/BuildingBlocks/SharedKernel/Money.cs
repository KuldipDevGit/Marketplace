namespace Marketplace.SharedKernel;

/// <summary>
/// A monetary amount in a specific ISO 4217 currency. An immutable value object compared by amount
/// and currency. Amounts are non-negative; arithmetic across different currencies is intentionally
/// not supported here (a Pricing/FX concern).
/// </summary>
public sealed class Money : ValueObject
{
    private Money()
    {
    }

    public Money(decimal amount, string currency)
    {
        if (amount < 0m)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Amount cannot be negative.");
        }

        if (string.IsNullOrWhiteSpace(currency) || currency.Length != 3)
        {
            throw new ArgumentException("Currency must be a 3-letter ISO 4217 code.", nameof(currency));
        }

        Amount = amount;
        Currency = currency.ToUpperInvariant();
    }

    public decimal Amount { get; private set; }
    public string Currency { get; private set; } = null!;

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }

    public override string ToString() => FormattableString.Invariant($"{Amount} {Currency}");
}
