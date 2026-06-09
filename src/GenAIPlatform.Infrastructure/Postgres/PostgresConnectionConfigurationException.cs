namespace GenAIPlatform.Infrastructure.Postgres;

internal sealed class PostgresConnectionConfigurationException : InvalidOperationException
{
    private PostgresConnectionConfigurationException(
        string message,
        bool isMissing)
        : base(message)
    {
        IsMissing = isMissing;
    }

    public bool IsMissing { get; }

    public static PostgresConnectionConfigurationException Missing(string connectionStringName)
    {
        return new PostgresConnectionConfigurationException(
            $"PostgreSQL connection string '{connectionStringName}' is not configured.",
            isMissing: true);
    }

    public static PostgresConnectionConfigurationException Invalid(string connectionStringName)
    {
        return new PostgresConnectionConfigurationException(
            $"PostgreSQL connection string '{connectionStringName}' is invalid.",
            isMissing: false);
    }
}
