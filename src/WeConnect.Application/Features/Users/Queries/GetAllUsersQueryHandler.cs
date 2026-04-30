using MediatR;
using WeConnect.Application.Common.Interfaces;
using WeConnect.Domain.Entities;
using WeConnect.Application.Common.Models;

namespace WeConnect.Application.Users.Queries.GetAllUsers;

public class GetAllUsersQueryHandler
    : IRequestHandler<GetAllUsersQuery, PagedList<User>>
{
    private readonly IUserRepository _repo;

    public GetAllUsersQueryHandler(IUserRepository repo)
    {
        _repo = repo;
    }

    public async Task<PagedList<User>> Handle(
        GetAllUsersQuery request,
        CancellationToken cancellationToken)
    {
        return await _repo.GetPagedAsync(
            request.Page,
            request.PageSize);
    }
}