namespace Ginecologia.Shared.Kernel;

public sealed record Result(bool IsSuccess, string? Error = null)
{
    public static Result Success() => new(true);
    public static Result Failure(string error) => new(false, error);
}

public abstract class DomainException(string message) : Exception(message);
