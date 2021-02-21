using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NETCoreTemplate.Domain.Entities;
using NETCoreTemplate.Domain.Interfaces;
using NETCoreTemplate.Domain.Interfaces.Services;
using NETCoreTemplate.Service.Base;

namespace NETCoreTemplate.Service
{
    public class WorkService: BaseService, IWorkService
    {
        public WorkService(Func<IUnitOfWork> unitOfWorkFactory) : base(unitOfWorkFactory)
        {
        }

        public async Task<IList<Work>> GetAll()
        {
            return await ExecuteTransaction(async unitOfWork => (await unitOfWork.Repository<Work>().GetAllAsync()).ToList()); 
        }

        public async Task<Work> GetOne(int workId)
        {
            return await UnitOfWork.Repository<Work>().FindAsync(workId);
        }

        public async Task Update(Work workInput)
        {
            await ExecuteTransaction(async unitOfWork =>
            {
                var workRepos = UnitOfWork.Repository<Work>();
                var work = await workRepos.FindAsync(workInput.Id);
                if (work == null) 
                    throw new KeyNotFoundException();

                work.Name = work.Name;
                await unitOfWork.SaveChangesAsync();
                return true;
            });
        }

        public async Task Add(Work workInput)
        {
            await ExecuteTransaction(async unitOfWork =>
            {
                var workRepos = UnitOfWork.Repository<Work>();
                await workRepos.InsertAsync(workInput);
                return true;
            });
        }

        public async Task Delete(int workId)
        {
            await ExecuteTransaction(async unitOfWork =>
            {
                var workRepos = UnitOfWork.Repository<Work>();
                var work = await workRepos.FindAsync(workId);
                if (work == null)
                    throw new KeyNotFoundException();

                await workRepos.DeleteAsync(work);
                return true;
            });
        }
    }
}
