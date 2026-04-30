using MediatR;
using WeConnect.Application.Common.Interfaces;
using WeConnect.Domain.Entities;
namespace WeConnect.Application.Users.Commands.CreateUser;
public class CreateUserCommandHandler 
    : IRequestHandler<CreateUserCommand, Guid>
{
    private readonly IUserRepository _repo;

    public CreateUserCommandHandler(IUserRepository repo)
    {
        _repo = repo;
    }

    public async Task<Guid> Handle(
        CreateUserCommand request,
        CancellationToken cancellationToken)
    {
        var nameParts = request.Name.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
        var firstName = nameParts.FirstOrDefault() ?? "";
        var lastName = nameParts.Length > 1 ? nameParts[1] : "";
        var user = User.Create(firstName, lastName, request.Email);

        await _repo.AddAsync(user);

        return user.Id;
    }
}