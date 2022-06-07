using eCompany.Models;
using eCompany.Models.DTOs.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCompany.DataAccess.Repository.IRepository
{
    public interface ITaskRepository : IRepository<TaskEntity>
    {
        public Task<IQueryable<TaskEntityDTO>> GetAllTasks(int id, Status? status);
        public Task<TaskEntityDTO> GetTaskDetails(int taskId);
        void Update(TaskEntity task);

    }
}
