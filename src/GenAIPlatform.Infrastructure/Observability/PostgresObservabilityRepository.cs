using GenAIPlatform.Application.Usage.GetUsage;
using GenAIPlatform.Domain.Observability;
using GenAIPlatform.Infrastructure.Postgres;
using Npgsql;

namespace GenAIPlatform.Infrastructure.Observability;

internal sealed class PostgresObservabilityRepository(PostgresDataSourceProvider dataSourceProvider)
    : IAiRequestLogRepository, IPricingRepository, IUsageRepository
{
    public async Task AddAsync(
        AiRequestLogEntry entry,
        CancellationToken cancellationToken)
    {
        await using var connection = await OpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand("""
            INSERT INTO genai.ai_request_logs (
                request_id, api_version, user_id, tenant_id, correlation_id,
                provider, model, status, error_code, latency_ms,
                input_tokens, output_tokens, total_tokens, embedding_tokens,
                estimated_cost, cost_currency,
                prompt_template_name, prompt_template_version, prompt_template_content_hash,
                retrieval_latency_ms, retrieved_document_ids, citation_references, created_at_utc)
            VALUES (
                @request_id, @api_version, @user_id, @tenant_id, @correlation_id,
                @provider, @model, @status, @error_code, @latency_ms,
                @input_tokens, @output_tokens, @total_tokens, @embedding_tokens,
                @estimated_cost, @cost_currency,
                @prompt_template_name, @prompt_template_version, @prompt_template_content_hash,
                @retrieval_latency_ms, @retrieved_document_ids, @citation_references, @created_at_utc);
            """, connection);

        AddParameter(command, "request_id", entry.RequestId);
        AddParameter(command, "api_version", entry.ApiVersion);
        AddParameter(command, "user_id", entry.UserId);
        AddParameter(command, "tenant_id", entry.TenantId);
        AddParameter(command, "correlation_id", entry.CorrelationId);
        AddParameter(command, "provider", entry.Provider);
        AddParameter(command, "model", entry.Model);
        AddParameter(command, "status", entry.Status);
        AddParameter(command, "error_code", entry.ErrorCode);
        AddParameter(command, "latency_ms", ToMilliseconds(entry.Latency));
        AddParameter(command, "input_tokens", entry.InputTokens);
        AddParameter(command, "output_tokens", entry.OutputTokens);
        AddParameter(command, "total_tokens", entry.TotalTokens);
        AddParameter(command, "embedding_tokens", entry.EmbeddingTokens);
        AddParameter(command, "estimated_cost", entry.EstimatedCost);
        AddParameter(command, "cost_currency", entry.CostCurrency);
        AddParameter(command, "prompt_template_name", entry.Prompt?.TemplateName);
        AddParameter(command, "prompt_template_version", entry.Prompt?.Version);
        AddParameter(command, "prompt_template_content_hash", entry.Prompt?.ContentHash);
        AddParameter(command, "retrieval_latency_ms", entry.RetrievalLatency is null ? null : ToMilliseconds(entry.RetrievalLatency.Value));
        AddParameter(command, "retrieved_document_ids", entry.RetrievedDocuments
            .Select(static reference => reference.DocumentId)
            .Distinct()
            .ToArray());
        AddParameter(command, "citation_references", entry.RetrievedDocuments
            .Select(static reference => reference.ReferenceId)
            .Where(static referenceId => !string.IsNullOrWhiteSpace(referenceId))
            .ToArray());
        AddParameter(command, "created_at_utc", entry.CreatedAtUtc);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task<PricingRecord?> GetEffectivePricingAsync(
        string provider,
        string model,
        DateTimeOffset usedAtUtc,
        CancellationToken cancellationToken)
    {
        await using var connection = await OpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand("""
            SELECT id, provider, model, currency, input_token_price_per_million,
                   output_token_price_per_million, embedding_token_price_per_million,
                   effective_from_utc, effective_to_utc
            FROM genai.ai_model_pricing
            WHERE provider = @provider
              AND model = @model
              AND effective_from_utc <= @used_at_utc
              AND (effective_to_utc IS NULL OR effective_to_utc > @used_at_utc)
            ORDER BY effective_from_utc DESC
            LIMIT 1;
            """, connection);
        AddParameter(command, "provider", provider);
        AddParameter(command, "model", model);
        AddParameter(command, "used_at_utc", usedAtUtc);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        return await reader.ReadAsync(cancellationToken)
            ? MapPricingRecord(reader)
            : null;
    }

    public async Task<UsageSummary> GetUsageAsync(
        UsageQuery query,
        CancellationToken cancellationToken)
    {
        await using var connection = await OpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand("""
            SELECT COUNT(*)::bigint,
                   COALESCE(SUM(input_tokens), 0)::bigint,
                   COALESCE(SUM(output_tokens), 0)::bigint,
                   COALESCE(SUM(embedding_tokens), 0)::bigint,
                   COALESCE(SUM(estimated_cost), 0)::numeric,
                   COUNT(DISTINCT cost_currency) FILTER (WHERE cost_currency IS NOT NULL)::integer,
                   COALESCE(MAX(cost_currency) FILTER (WHERE cost_currency IS NOT NULL), 'USD')::text
            FROM genai.ai_request_logs
            WHERE (@from_utc::timestamp with time zone IS NULL OR created_at_utc >= @from_utc)
              AND (@to_utc::timestamp with time zone IS NULL OR created_at_utc <= @to_utc)
              AND (@user_id::text IS NULL OR user_id = @user_id)
              AND (@tenant_id::text IS NULL OR tenant_id = @tenant_id)
              AND (@model::text IS NULL OR model = @model);
            """, connection);
        AddParameter(command, "from_utc", query.FromUtc);
        AddParameter(command, "to_utc", query.ToUtc);
        AddParameter(command, "user_id", NormalizeFilter(query.UserId));
        AddParameter(command, "tenant_id", NormalizeFilter(query.TenantId));
        AddParameter(command, "model", NormalizeFilter(query.Model));

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        if (!await reader.ReadAsync(cancellationToken))
        {
            return new UsageSummary(0, 0, 0, 0, 0, "USD");
        }

        if (reader.GetInt32(5) > 1)
        {
            throw new UsageQueryValidationException(
                "Usage query spans multiple cost currencies. Narrow the query before aggregating estimated cost.");
        }

        return new UsageSummary(
            reader.GetInt64(0),
            reader.GetInt64(1),
            reader.GetInt64(2),
            reader.GetInt64(3),
            reader.GetDecimal(4),
            reader.GetString(6));
    }

    private async Task<NpgsqlConnection> OpenConnectionAsync(CancellationToken cancellationToken)
    {
        return await dataSourceProvider.OpenConnectionAsync(cancellationToken);
    }

    private static PricingRecord MapPricingRecord(NpgsqlDataReader reader)
    {
        return new PricingRecord(
            reader.GetGuid(0),
            reader.GetString(1),
            reader.GetString(2),
            reader.GetString(3),
            reader.GetDecimal(4),
            reader.GetDecimal(5),
            reader.IsDBNull(6) ? null : reader.GetDecimal(6),
            GetDateTimeOffset(reader, 7),
            reader.IsDBNull(8) ? null : GetDateTimeOffset(reader, 8));
    }

    private static DateTimeOffset GetDateTimeOffset(NpgsqlDataReader reader, int ordinal)
    {
        var value = reader.GetDateTime(ordinal);
        return value.Kind == DateTimeKind.Utc
            ? new DateTimeOffset(value)
            : new DateTimeOffset(DateTime.SpecifyKind(value, DateTimeKind.Utc));
    }

    private static int ToMilliseconds(TimeSpan value)
    {
        return Math.Max(0, (int)Math.Ceiling(value.TotalMilliseconds));
    }

    private static string? NormalizeFilter(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    private static void AddParameter(NpgsqlCommand command, string name, object? value)
    {
        command.Parameters.AddWithValue(name, value ?? DBNull.Value);
    }
}
