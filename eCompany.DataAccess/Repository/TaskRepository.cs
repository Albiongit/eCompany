using AutoMapper;
using eCompany.DataAccess.Data;
using eCompany.DataAccess.Repository.IRepository;
using eCompany.Models;
using eCompany.Models.DTOs.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
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

            if(status == null)
            {
                var taskDetails = (from t in _db.Tasks
                                   join c in _db.Companies on t.CompanyID equals c.CompanyId
                                   join aU in _db.ApplicationUsers on t.EmployeeId equals aU.Id
                                   where c.CompanyId == id
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
                                       Status = t.Status,
                                       StatusInfo = t.Status.ToString()
                                   }).AsQueryable();

                return taskDetails;

            }else
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
                                       Status = t.Status,
                                       StatusInfo = t.Status.ToString()
                                   }).AsQueryable();

                return taskDetails;
            }

            
        }

        public async Task<TaskEntityDTO> GetTaskDetails(int taskId)
        {
            var taskDetails = (from t in _db.Tasks
                               join c in _db.Companies on t.CompanyID equals c.CompanyId
                               join aU in _db.ApplicationUsers on t.EmployeeId equals aU.Id
                               where t.TaskId == taskId
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
                                   DueDate = t.AssignedDate.AddDays(t.DayDuration),
                                   EmployeeName = aU.Name,
                                   Comment = t.Comment,
                                   Status = t.Status
                               }).FirstOrDefault();

            return taskDetails;
        }



        public async Task<IQueryable<TaskEntityDTO>> GetEmployeeTasks(string id, Status? status)
        {

            if (status == null)
            {
                var taskDetails = (from t in _db.Tasks
                                   join c in _db.Companies on t.CompanyID equals c.CompanyId
                                   join aU in _db.ApplicationUsers on t.EmployeeId equals aU.Id
                                   where aU.Id == id
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
                                       DueDate = t.AssignedDate.AddDays(t.DayDuration),
                                       DueDateTask = t.AssignedDate.AddDays(t.DayDuration).ToString("MMMM dd, yyyy"),
                                       EmployeeName = aU.Name,
                                       Status = t.Status,
                                       StatusInfo = t.Status.ToString()
                                   }).AsQueryable();

                return taskDetails;

            }
            else
            {
                var taskDetails = (from t in _db.Tasks
                                   join c in _db.Companies on t.CompanyID equals c.CompanyId
                                   join aU in _db.ApplicationUsers on t.EmployeeId equals aU.Id
                                   where aU.Id == id
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
                                       DueDate = t.AssignedDate.AddDays(t.DayDuration),
                                       DueDateTask = t.AssignedDate.AddDays(t.DayDuration).ToString("MMMM dd, yyyy"),
                                       EmployeeName = aU.Name,
                                       Status = t.Status,
                                       StatusInfo = t.Status.ToString()
                                   }).AsQueryable();

                return taskDetails;
            }


        }

        public async Task<IQueryable<TaskEntityDTO>> GetEmployeeMonthlyTasks(string id)
        {

                var taskDetails = (from t in _db.Tasks
                                   join c in _db.Companies on t.CompanyID equals c.CompanyId
                                   join aU in _db.ApplicationUsers on t.EmployeeId equals aU.Id
                                   where aU.Id == id
                                   where t.AssignedDate >= DateTime.Now.AddDays(-30)
                                   select new TaskEntityDTO
                                   {
                                       CompanyId = t.Company.CompanyId,
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
                                       DueDate = t.AssignedDate.AddDays(t.DayDuration),
                                       DueDateTask = t.AssignedDate.AddDays(t.DayDuration).ToString("MMMM dd, yyyy"),
                                       EmployeeName = aU.Name,
                                       Status = t.Status,
                                       StatusInfo = t.Status.ToString()
                                   }).AsQueryable();

                return taskDetails;


        }




        public void Update(TaskEntity task)
        {
             _db.Update(task);
        }
    }
}
