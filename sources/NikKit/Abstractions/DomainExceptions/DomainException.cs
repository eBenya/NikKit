using System.Text.RegularExpressions;

namespace NikKit.DomainExceptions;

/// <summary>
/// Serves as the base class for all domain-specific exceptions.
/// Provides automatic ErrorCode generation and a structured context dictionary 
/// for logging and telemetry.
/// </summary>
public abstract class DomainException : Exception
{
    private readonly Dictionary<string, object> _contextData = new();

    /// <summary>
    /// The unique error identifier. 
    /// Automatically generated as SNAKE_CASE based on the class name (e.g., UserNotFoundException -> USER_NOT_FOUND).
    /// </summary>
    public string ErrorCode { get; }

    /// <summary>
    /// Additional metadata associated with the error, useful for structured logging, monitoring, and API responses.
    /// </summary>
    public IReadOnlyDictionary<string, object> ContextData => _contextData;

    protected DomainException(string message, Exception? innerException = null) 
        : base(message, innerException)
    {
        ErrorCode = GenerateErrorCode(GetType().Name);
    }

    protected DomainException(string errorCode, string message, Exception? innerException = null) 
        : base(message, innerException)
    {
        ErrorCode = errorCode;
    }

    /// <summary>
    /// Adds a key-value pair to the exception context.
    /// Used by inheriting classes to provide diagnostic metadata.
    /// </summary>
    protected void AddData(string key, object value)
    {
        _contextData[key] = value;
    }

    /// <summary>
    /// Convert class name(InsufficientFundsException) 
    /// to exception code(INSUFFICIENT_FUNDS).
    /// </summary>
    private static string GenerateErrorCode(string className)
    {
        var exceptionWordLen = "Exception".Length;
        var name = className.EndsWith("Exception") 
            ? className[..^exceptionWordLen] 
            : className;
        
        // Transform PascalCase to SNAKE_CASE
        return Regex.Replace(name, "([a-z])([A-Z])", "$1_$2").ToUpperInvariant();
    }
}