using FunkyOrders.Core.Shared;

namespace FunkyOrders.Features.CreateOrder;

public interface IOrderProcessor
{
    Task<OperationResponse<OperationResult.FailedResult, OperationResult.SuccessResult<OrderAcceptedData>>> ProcessAsync(
        CreateOrderRequestDto request,
        CancellationToken token
    );
}
