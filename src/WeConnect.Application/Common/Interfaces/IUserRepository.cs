using WeConnect.Domain.Entities;
namespace WeConnect.Application.Common.Interfaces;
using WeConnect.Application.Common.Models;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);

    Task<PagedList<User>> GetPagedAsync(
        int page,
        int pageSize);
    Task AddAsync(User user);

    Task UpdateAsync(User user);

    Task DeleteAsync(User user);
}