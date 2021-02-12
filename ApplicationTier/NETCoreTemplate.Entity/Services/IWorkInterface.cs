using System.Collections.Generic;
using System.Threading.Tasks;
using NETCoreTemplate.Entity.Entities;

namespace NETCoreTemplate.Entity.Services
{
    public interface IWorkService
    {
        Task<IList<Work>> GetAll();
        Task<Work> GetOne(int workId);
        Task Update(Work work);
        Task Add(Work work);
        Task Delete(int workId);
    }
}
