using BT_DataModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BT_DataAccess.Interfaces
{
    public interface IRepository
    {
        Task<Job> GetById(int id);
        Task<int> Create(Job entity);
        Task<bool> Delete(int id);
        Task<int> Update(Job entity);
        Task<List<Job>> GetAll();
    }
}
