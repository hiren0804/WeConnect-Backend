using MediatR;
using Microsoft.AspNetCore.Mvc;
using WeConnect.Application.Common.Models;

namespace WeConnect.API.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
public abstract class BaseApiController : ControllerBase
{
    private ISender? _sender;
    protected ISender Sender =>
        _sender ??= HttpContext.RequestServices.GetRequiredService<ISender>();

    protected IActionResult OkResponse<T>(T data, string message = "Success", object? meta = null)
        => Ok(ApiResponse<T>.Ok(data, message, meta));

    protected IActionResult CreatedResponse<T>(T data, string routeName, object routeValues)
        => CreatedAtRoute(routeName, routeValues,
               ApiResponse<T>.Ok(data, "Resource created successfully"));

    protected IActionResult NoContentResponse(string message = "Done")
        => Ok(ApiResponse.NoContent(message));
}