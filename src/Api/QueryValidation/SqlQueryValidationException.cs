namespace Api.QueryValidation;

public sealed class SqlQueryValidationException : Exception
{
    public SqlQueryValidationException(string message)
        : base(message)
    {
    }
}
