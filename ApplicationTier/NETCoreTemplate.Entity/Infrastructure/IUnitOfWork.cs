using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace NETCoreTemplate.Entity.Infrastructure
{
    public interface IUnitOfWork : IDisposable
    {
        DbContext DbContext { get; }

        IRepository<T> Repository<T>() where T : class;

        /// <summary>
        /// Saves changes to database, previously opening a transaction
        /// only when none exists. The transaction is opened with isolation
        /// level set in Unit of Work before calling this method.
        /// </summary>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
