namespace GenAIPlatform.Api.Configuration;

public sealed class DemoAuthOptions
{
    public const string SectionName = "GenAIPlatform:DemoAuth";

    public bool Enabled { get; init; } = true;

    public bool AllowInNonDevelopment { get; init; }

    public string? DefaultUserId { get; init; } = "demo-user";

    public string? DefaultTenantId { get; init; } = "demo-tenant";

    public string[] DefaultRoles { get; init; } = ["developer"];

    public string[] DefaultGroups { get; init; } = ["demo"];
}
