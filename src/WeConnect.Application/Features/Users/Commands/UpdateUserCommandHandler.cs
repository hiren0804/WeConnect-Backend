using System;
using System.Linq;
using MediatR;
using WeConnect.Application.Common.Exceptions;
using WeConnect.Application.Common.Interfaces;
using WeConnect.Domain.Entities;
namespace WeConnect.Application.Users.Commands.UpdateUser;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, bool>
{
    private readonly IUserRepository _repo;

    public UpdateUserCommandHandler(IUserRepository repo)
    {
        _repo = repo;
    }

    public async Task<bool> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _repo.GetByIdAsync(request.Id)
            ?? throw new NotFoundException($"User {request.Id} not found");

        var nameParts = request.Name.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
        var firstName = nameParts.FirstOrDefault() ?? "";
        var lastName = nameParts.Length > 1 ? nameParts[1] : "";
        user.UpdateProfile(firstName, lastName, null);  // Skip email (private setter)

        await _repo.UpdateAsync(user);

        return true;
    }
}
