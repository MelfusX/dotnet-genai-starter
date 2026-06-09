using GenAIPlatform.Application.Evaluations.StartRun;
using GenAIPlatform.Application.Core.Dispatching;

namespace GenAIPlatform.Application.Evaluations;

public sealed record GetEvaluationRunQuery(Guid RunId)
    : IRequest<EvaluationRunResult?>;
