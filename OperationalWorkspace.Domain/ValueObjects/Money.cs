using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Linq;
using OperationalWorkspace.Domain.Exceptions;
using Bogus.DataSets;


namespace OperationalWorkspace.Domain.ValueObjects;

public record Money
{
    public decimal Amount { get; init; }
    public string Currency { get; init; }

    public Money(decimal amount, string currency = "USD")
    {
        if (amount < 0) throw new ArgumentException("Amount cannot be negative");

        Amount = amount;
        Currency = currency?.ToUpper() ?? "USD";
    }

    public static Money operator +(Money a, Money b) =>
        a.Currency != b.Currency
            ? throw new InvalidOperationException("Currency mismatch")
            : new Money(a.Amount + b.Amount, a.Currency);

    public override string ToString() => $"{Currency} {Amount:N2}";
}
