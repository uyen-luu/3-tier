using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NETCoreTemplate.Domain.Interfaces;

namespace NETCoreTemplate.Infrastructure
{
	public class UnitOfWork : IUnitOfWork
	{
		public DbContext DbContext { get; private set; }
        private Dictionary<string, object> Repositories { get; }

        public UnitOfWork(DbContext dbContext)
		{
			DbContext = dbContext;
            Repositories = new Dictionary<string, dynamic>();
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
		{
			return await DbContext.SaveChangesAsync(cancellationToken);
		}


		public void Dispose()
		{
			if (DbContext == null)
				return;
			//
			// Close connection
			if (DbContext.Database.GetDbConnection().State == ConnectionState.Open)
			{
				DbContext.Database.GetDbConnection().Close();
			}
			DbContext.Dispose();

			DbContext = null;
		}

        public IRepository<TEntity> Repository<TEntity>() where TEntity : class
		{
			var type = typeof(TEntity);
			var typeName = type.Name;

			lock (Repositories)
			{
				if (Repositories.ContainsKey(typeName))
                {
                    return (IRepository<TEntity>) Repositories[typeName];
                }

                var repository = new Repository<TEntity>(DbContext);

				Repositories.Add(typeName, repository);
				return repository;
			}
		}
    }
}
