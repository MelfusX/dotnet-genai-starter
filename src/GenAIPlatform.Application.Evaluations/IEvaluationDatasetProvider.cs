using GenAIPlatform.Domain.Evaluations;

namespace GenAIPlatform.Application.Evaluations;

public interface IEvaluationDatasetProvider
{
    Task<EvaluationDataset> GetDatasetAsync(
        string? datasetVersion,
        CancellationToken cancellationToken);
}
