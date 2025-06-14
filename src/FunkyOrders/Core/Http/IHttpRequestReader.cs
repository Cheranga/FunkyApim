using FunkyOrders.Core.Shared;
using Microsoft.Azure.Functions.Worker.Http;

namespace FunkyOrders.Core.Http;

public interface IHttpRequestReader
{
    Task<OperationResponse<OperationResult.FailedResult, OperationResult.SuccessResult<TRequest>>> ReadRequestAsync<TRequest>(
        HttpRequestData request,
        CancellationToken token
    )
        where TRequest : class;
}
