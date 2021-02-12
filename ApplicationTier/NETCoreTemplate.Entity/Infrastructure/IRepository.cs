using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace NETCoreTemplate.Entity.Infrastructure
{
	public interface IRepository<T> where T : class
    {
        DbSet<T> Entities { get; }
        DbContext DbContext { get; }

        Task<IEnumerable<T>> GetAllAsync();

        T Find(params object[] keyValues);
        Task<T> FindAsync(params object[] keyValues);

        Task InsertAsync(T entity, bool saveChanges = true);
        Task InsertRangeAsync(IEnumerable<T> entities, bool saveChanges = true);

        Task DeleteAsync(int id, bool saveChanges = true);
        Task DeleteAsync(T entity, bool saveChanges = true);

        Task DeleteRangeAsync(IEnumerable<T> entities, bool saveChanges = true);
    }
}
