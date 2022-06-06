using eCompany.DataAccess.Data;
using eCompany.DataAccess.Repository.IRepository;
using eCompany.Models;
using eCompany.Models.DTOs.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCompany.DataAccess.Repository
{
    public class TaskRepository : Repository<TaskEntity>, ITaskRepository
    {
        private readonly ApplicationDbContext _db;

        public TaskRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }


        public async Task<IQueryable<TaskEntityDTO>> GetAllTasks(int id, Status? status)
        {

            var taskDetails = (from t in _db.Tasks
                               join c in _db.Companies on t.CompanyID equals c.CompanyId
                               join aU in _db.ApplicationUsers on t.EmployeeId equals aU.Id
                               where c.CompanyId == id
                               where t.Status == status
                               select new TaskEntityDTO
                               {
                                   CompanyId = c.CompanyId,
                                   CompanyName = c.CompanyName,
                                   CompanyPhone = c.CompanyPhone,
                                   CompanyState = c.CompanyState,
                                   CompanyWeb = c.CompanyWeb,
                                   TaskId = t.TaskId,
                                   Title = t.Title,
                                   EmployeeId = t.EmployeeId,
                                   DayDuration = t.DayDuration,
                                   Description = t.Description,
                                   AssignedDate = t.AssignedDate,
                                   FinishedDate = t.FinishedDate,
                                   EmployeeName = aU.Name,
                                   Status = t.Status
                               }).AsQueryable();

            return taskDetails;
        }


        public void Update(TaskEntity task)
        {
             _db.Update(task);
        }
    }
}
