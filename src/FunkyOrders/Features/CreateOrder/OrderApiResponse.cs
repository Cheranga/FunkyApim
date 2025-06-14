using Microsoft.Azure.Functions.Worker.Http;

namespace FunkyOrders.Features.CreateOrder;

public sealed record OrderApiResponse
{
    public HttpResponseData? HttpResponse { get; set; }
}
