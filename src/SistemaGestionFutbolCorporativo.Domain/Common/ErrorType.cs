namespace SistemaGestionFutbolCorporativo.Domain.Common;

public enum ErrorType
{
    Failure = 0,
    Validation = 1,
    NotFound = 2,
    Conflict = 3
}


public readonly record struct Error(string Message, string? Code = null, ErrorType Type = ErrorType.Failure)
{
    public static readonly Error None = new(string.Empty, null, ErrorType.Failure);

    public static Error Failure(string message, string? code = null) => 
        new(message, code, ErrorType.Failure);

    public static Error Validation(string message, string? code = null) => 
        new(message, code, ErrorType.Validation);

    public static Error NotFound(string message, string? code = null) => 
        new(message, code, ErrorType.NotFound);

    public static Error Conflict(string message, string? code = null) => 
        new(message, code, ErrorType.Conflict);
}