using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WeConnect.Application.Common.Models;
using WeConnect.Application.Users.Commands.CreateUser;
using WeConnect.Application.Users.Commands.UpdateUser;
using WeConnect.Application.Users.Commands.DeleteUser;
using WeConnect.Application.Users.Queries.GetAllUsers;
using WeConnect.Application.Users.Queries.GetUserById;

namespace WeConnect.API.Controllers;

[Authorize]
public class UsersController : BaseApiController
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>Create new user</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<Guid>), 200)]
    public async Task<IActionResult> CreateUser(
        [FromBody] CreateUserCommand command)
    {
        var userId = await _mediator.Send(command);

        return OkResponse(userId, "User created successfully");
    }

    /// <summary>Get all users</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<object>>), 200)]
    public async Task<IActionResult> GetUsers(
       [FromQuery] PaginationRequest request)
    {
        var pagedUsers = await _mediator.Send(
            new GetAllUsersQuery(
                request.ValidPage,
                request.ValidPageSize));

        return OkResponse(
            pagedUsers.Items,
            "Users fetched successfully",
            pagedUsers.ToMeta());
    }

    /// <summary>Get user by id</summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    public async Task<IActionResult> GetUserById(Guid id)
    {
        var user = await _mediator.Send(new GetUserByIdQuery(id));

        return OkResponse(user, "User fetched successfully");
    }

    /// <summary>Update user</summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    public async Task<IActionResult> UpdateUser(
        Guid id,
        [FromBody] UpdateUserCommand command)
    {
        command.Id = id;

        var result = await _mediator.Send(command);

        return OkResponse(result, "User updated successfully");
    }

    /// <summary>Delete user</summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        var result = await _mediator.Send(
            new DeleteUserCommand(id));

        return OkResponse(result, "User deleted successfully");
    }

    /// <summary>Get current authenticated user profile</summary>
    [HttpGet("me")]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    public IActionResult GetMe()
    {
        var userId = User.FindFirst("oid")?.Value
                  ?? User.FindFirst("sub")?.Value;

        var email = User.FindFirst("preferred_username")?.Value
                  ?? User.FindFirst("email")?.Value;

        var name = User.FindFirst("name")?.Value;

        var profile = new
        {
            userId,
            email,
            name
        };

        return OkResponse(profile, "Profile fetched successfully");
    }
}