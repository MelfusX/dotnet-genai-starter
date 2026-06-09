using GenAIPlatform.Application.Core.Dispatching;

namespace GenAIPlatform.Application.Evaluations;

public sealed record GetEvaluationSummaryQuery(Guid RunId)
    : IRequest<EvaluationRunSummary?>;
