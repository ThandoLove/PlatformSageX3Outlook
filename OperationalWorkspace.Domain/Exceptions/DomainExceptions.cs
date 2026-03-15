

namespace OperationalWorkspace.Domain.Exceptions;

// Keep abstract so it's only a base for other exceptions
public abstract class DomainException : Exception
{
    protected DomainException(string msg) : base(msg) { }
}

public class BusinessRuleException : DomainException
{
    public BusinessRuleException(string msg) : base(msg) { }
}

public class CreditLimitExceededException : DomainException
{
    public CreditLimitExceededException(string bp, decimal req, decimal lim)
        : base($"BP {bp} credit limit exceeded. Requested: {req:N2}, Limit: {lim:N2}") { }
}

public class InsufficientStockException : DomainException
{
    public InsufficientStockException(string item, int req, int avail)
        : base($"Insufficient stock for {item}. Requested: {req}, Available: {avail}") { }
}


