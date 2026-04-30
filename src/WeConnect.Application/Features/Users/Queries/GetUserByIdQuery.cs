using MediatR;
using WeConnect.Domain.Entities;

public record GetUserByIdQuery(Guid Id) : IRequest<User>;