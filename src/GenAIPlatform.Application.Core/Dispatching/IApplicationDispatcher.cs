namespace GenAIPlatform.Application.Core.Dispatching;

public interface IApplicationDispatcher
{
    Task<TResponse> DispatchAsync<TRequest, TResponse>(
        TRequest request,
        CancellationToken cancellationToken = default)
        where TRequest : IRequest<TResponse>;
}
