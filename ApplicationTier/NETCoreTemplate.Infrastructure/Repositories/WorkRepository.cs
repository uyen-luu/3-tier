using System.Collections.Generic;
using System.Threading.Tasks;
using NETCoreTemplate.Domain.Entities;
using NETCoreTemplate.Domain.Interfaces;
using NETCoreTemplate.Domain.Utilities;

namespace NETCoreTemplate.Infrastructure.Repositories
{
    public static class WorkRepository
    {
        public static async Task<IList<Work>> GetAll(this IRepository<Work> repository)
        {
            var works = new List<Work>();
            await repository.DbContext.LoadStoredProc("spGetWorks")
                .ExecuteStoredProcAsync(result =>
                {
                    works = result.ReadNextListOrEmpty<Work>();
                });

            return works;
        }
    }
}
