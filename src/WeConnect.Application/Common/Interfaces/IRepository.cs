using WeConnect.Domain.Common;

namespace WeConnect.Application.Common.Interfaces;

public interface IRepository<T> where T : BaseEntity
{
    void Update(T entity);
    void Delete(T entity);
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
