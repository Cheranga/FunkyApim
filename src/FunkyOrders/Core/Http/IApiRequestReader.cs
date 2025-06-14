using FluentValidation;
using FunkyOrders.Core.Shared;
using Microsoft.Azure.Functions.Worker.Http;

namespace FunkyOrders.Core.Http;

public interface IApiRequestReader<TDto, TDtoValidator>
    where TDto : class, IApiRequestDto<TDto, TDtoValidator>
    where TDtoValidator : class, IValidator<TDto>
{
    Task<OperationResponse<OperationResult.FailedResult, OperationResult.SuccessResult<TDto>>> ReadRequestAsync(
        HttpRequestData request,
        CancellationToken token
    );
}
