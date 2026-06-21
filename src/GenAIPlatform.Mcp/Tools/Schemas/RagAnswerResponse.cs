namespace GenAIPlatform.Mcp.Tools.Schemas;

public sealed record RagAnswerResponse(
    string Message,
    bool NoContext,
    string CorrelationId,
    IReadOnlyList<RagAnswerCitation> Citations);
