using System.Collections.Generic;
using System.Threading.Tasks;
using NETCoreTemplate.Entity.Entities;
using NETCoreTemplate.Entity.Infrastructure;
using NETCoreTemplate.Entity.Utilities;

namespace NETCoreTemplate.Repository
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
