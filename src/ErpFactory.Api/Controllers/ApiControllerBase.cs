using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ErpFactory.Api.Contracts;

namespace ErpFactory.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public abstract class ApiControllerBase : ControllerBase
{
    protected ActionResult<ApiResponse<T>> OkResponse<T>(T data, string message = "Operation completed successfully") =>
        Ok(ApiResponse<T>.Ok(data, message));

    protected ActionResult<ApiResponse<IReadOnlyCollection<T>>> OkCollection<T>(
        IReadOnlyCollection<T> data,
        string message = "Operation completed successfully") =>
        Ok(ApiResponse<IReadOnlyCollection<T>>.Ok(data, message));

    protected ActionResult<ApiResponse<T>> CreatedResponse<T>(string routeName, object values, T data) =>
        CreatedAtRoute(routeName, values, ApiResponse<T>.Ok(data, "Resource created successfully"));

    protected ActionResult<ApiResponse<T>> NotFoundResponse<T>(string message = "Resource was not found") =>
        NotFound(ApiResponse<T>.Fail(message));

    protected ActionResult<ApiResponse<T>> FailResponse<T>(string message, IReadOnlyCollection<string>? errors = null) =>
        BadRequest(ApiResponse<T>.Fail(message, errors));
}