using System.Net;
using System.Net.Mime;
using System.Text.Json;
using FunkyOrders.Core.Http;
using FunkyOrders.Core.Shared;
using Microsoft.Azure.Functions.Worker.Http;

namespace FunkyOrders.Features.CreateOrder;

internal sealed class OrderApiResponseGenerator(JsonSerializerOptions serializerOptions) : IOrderApiResponseGenerator
{
    public async Task<OrderApiResponse> GenerateOrderAcceptedResponseAsync(HttpRequestData request, Guid orderId, CancellationToken token)
    {
        var httpResponse = request.CreateResponse(HttpStatusCode.Accepted);
        httpResponse.Headers.Add("Content-Type", MediaTypeNames.Application.Json);
        var responseData = new OrderAcceptedData(orderId);
        await JsonSerializer.SerializeAsync(httpResponse.Body, responseData, serializerOptions, token);

        return new OrderApiResponse { HttpResponse = httpResponse };
    }

    public async Task<OrderApiResponse> GenerateErrorResponseAsync(
        HttpRequestData request,
        OperationResult.FailedResult failure,
        HttpStatusCode statusCode,
        CancellationToken token
    )
    {
        var httpResponse = request.CreateResponse(statusCode);
        httpResponse.Headers.Add("Content-Type", MediaTypeNames.Application.Json);
        await JsonSerializer.SerializeAsync(httpResponse.Body, failure.Error, serializerOptions, token);

        return new OrderApiResponse { HttpResponse = httpResponse };
    }
}
