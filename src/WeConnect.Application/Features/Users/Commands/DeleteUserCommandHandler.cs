using System;
using MediatR;
using WeConnect.Application.Common.Exceptions;
using WeConnect.Application.Common.Interfaces;
using WeConnect.Domain.Entities;
namespace WeConnect.Application.Users.Commands.DeleteUser;

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, bool>
{
    private readonly IUserRepository _repo;

    public DeleteUserCommandHandler(IUserRepository repo)
    {
        _repo = repo;
    }

    public async Task<bool> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _repo.GetByIdAsync(request.Id)
            ?? throw new NotFoundException($"User {request.Id} not found");

        await _repo.DeleteAsync(user);

        return true;
    }
}
