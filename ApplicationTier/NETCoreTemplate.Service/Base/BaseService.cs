using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NETCoreTemplate.Domain.Interfaces;

namespace NETCoreTemplate.Service.Base
{
    public abstract class BaseService : IDisposable
    {
        protected readonly Func<IUnitOfWork> UnitOfWorkFactory;

        /// <summary>
        /// This UnitOfWork only available inside the ExecuteTransaction methods
        /// </summary>
        protected IUnitOfWork UnitOfWork { get; private set; }

        protected IRepository<T> Repository<T>() where T : class
        {
            return UnitOfWork.Repository<T>();
        }

        protected async Task<T> InsertAsync<T>(T entity) where T : class
        {
            await UnitOfWork.Repository<T>().InsertAsync(entity);

            return entity;
        }

        protected async Task<IEnumerable<T>> InsertRangeAsync<T>(IEnumerable<T> entities) where T : class
        {
            var insertRangeAsync = entities as T[] ?? entities.ToArray();
            await UnitOfWork.Repository<T>().InsertRangeAsync(insertRangeAsync);

            return insertRangeAsync;
        }

        protected BaseService(Func<IUnitOfWork> unitOfWorkFactory)
        {
            UnitOfWorkFactory = unitOfWorkFactory;
        }

        /// <summary>
        /// Resolve unitOfWork from factory and execute the action inside a new resilient transaction which is automatically retries when failed
        /// </summary>
        /// <typeparam name="T">The return type</typeparam>
        /// <param name="action">The action</param>
        /// <param name="isolationLevel">The Isolation Level</param>
        /// <returns>Value return from the action</returns>
        protected async Task<T> ExecuteTransaction<T>(Func<IUnitOfWork, Task<T>> action, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            if (UnitOfWork != null)
            {
                throw new Exception(nameof(UnitOfWork));
            }

            T result = default;
            //
            // Get Unit Of Work to reduce code
            using (var unitOfWork = UnitOfWorkFactory.Invoke())
            {
                //
                // Set unit of work for the service to use across methods
                UnitOfWork = unitOfWork;
                //
                // Create Execution Strategy
                var db = UnitOfWork.DbContext.Database; 
                
                var strategy = db.CreateExecutionStrategy();
                //
                // Execute action in a resilient transaction for automatic retry if failed
                await strategy.ExecuteAsync(async () =>
                {
                    await using var transaction = await db.BeginTransactionAsync(isolationLevel);
                    try
                    {

                        result = await action(unitOfWork);

                        await transaction.CommitAsync();
                    }
                    catch (Exception)
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                });
            }

            //
            // Clean up unit of work because the transaction is finished
            UnitOfWork = null;

            return result;
        }


        public void Dispose()
        {
            UnitOfWork?.Dispose();
        }
    }
}
