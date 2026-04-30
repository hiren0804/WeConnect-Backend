using Microsoft.EntityFrameworkCore;
using WeConnect.Application.Common.Interfaces;
using WeConnect.Domain.Entities;
using WeConnect.Infrastructure.Persistence;
using WeConnect.Application.Common.Models;

namespace WeConnect.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly MasterDbContext _context;

    public UserRepository(MasterDbContext context)
    {
        _context = context;
    }
    public async Task<PagedList<User>> GetPagedAsync(
        int page,
        int pageSize)
    {
        var query = _context.Users.AsQueryable();

        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedList<User>(
            items,
            page,
            pageSize,
            totalCount);
    }
    public async Task<List<User>> GetAllAsync()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(User user)
    {
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
    }
}