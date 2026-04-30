using MediatR;
using WeConnect.Application.Common.Models;
using WeConnect.Domain.Entities;

public record GetAllUsersQuery(
    int Page = 1,
    int PageSize = 10
) : IRequest<PagedList<User>>;