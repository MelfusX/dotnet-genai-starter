namespace GenAIPlatform.Infrastructure.Configuration;

public sealed class PostgresOptions
{
    public const string SectionName = "GenAIPlatform:Postgres";

    public string ConnectionStringName { get; init; } = "GenAIPlatform";
}
