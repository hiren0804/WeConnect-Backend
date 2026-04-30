using System;
using MediatR;
using WeConnect.Application.Common.Exceptions;
using WeConnect.Application.Common.Interfaces;
using WeConnect.Domain.Entities;
namespace WeConnect.Application.Users.Queries.GetUserById;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, User>
{
    private readonly IUserRepository _repo;

    public GetUserByIdQueryHandler(IUserRepository repo)
    {
        _repo = repo;
    }

    public async Task<User> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _repo.GetByIdAsync(request.Id);
        return user ?? throw new NotFoundException($"User {request.Id} not found");
    }
}
