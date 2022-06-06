﻿using eCompany.DataAccess.Data;
using eCompany.DataAccess.Repository.IRepository;
using eCompany.Models;
using eCompany.Models.DTOs.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Authorization;
using eCompany.Shared;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace eCompany.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TaskController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _db;

        public TaskController(IUnitOfWork unitOfWork, ApplicationDbContext db)
        {
            _unitOfWork = unitOfWork;
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);


            var companyFromDb = await _unitOfWork.CompanyUsers.GetCompanyDetails(claim.Value);
            if ( companyFromDb != null)
            {
                return View(companyFromDb);
            }

            return View();
        }

        [HttpPost]
        public async Task<JsonResult> GetTaskList(int id, Status? status)
        {
            //var claimsIdentity = (ClaimsIdentity)User.Identity;
            //var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            //var CompanyUsers = await _unitOfWork.CompanyUsers.GetFirstOrDefaultAsync(uc => uc.UserId == claim.Value);
            //var CompanyId = CompanyUsers.CompanyId;

            int totalRecord = 0;
            int filterRecord = 0;
            var draw = Request.Form["draw"].FirstOrDefault();
            var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
            var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
            var searchValue = Request.Form["search[value]"].FirstOrDefault();
            int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
            int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");
            var data = await _unitOfWork.Tasks
                .GetAllTasks(id, status);                  // check function -- TO DO --

            totalRecord = data.Count();

            if (!string.IsNullOrEmpty(searchValue))
            {
                data = data.Where(x => x.TaskId == Int32.Parse(searchValue) || 
                                  x.EmployeeName.ToLower().Contains(searchValue.ToLower()) ||
                                  x.Title.ToLower().Contains(searchValue.ToLower()));
            }

            filterRecord = data.Count();

            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDirection))
            {
                data = data.OrderBy(sortColumn + " " + sortColumnDirection);
            }


            var taskList = data.Skip(skip).Take(pageSize).ToList();


            var returnObj = new
            {
                draw = draw,
                recordsTotal = totalRecord,
                recordsFiltered = filterRecord,
                data = taskList
            };

            return Json(returnObj);
        }



        //Create Task - GET
        [HttpGet]
        [Authorize(Roles = SD.Role_SuperAdmin + "," + SD.Role_Admin)]
        public async Task<IActionResult> CreateTask(int companyId)
        {
            TaskEntityDTO taskEntityDTO = new TaskEntityDTO();
            taskEntityDTO.CompanyId = companyId;

            var getAllEmployees = await _unitOfWork.CompanyUsers.GetAllUsers(companyId, "Employee");

            taskEntityDTO.EmployeeList = getAllEmployees.Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            });

            var company = await _unitOfWork.Company.GetFirstOrDefaultAsync(c => c.CompanyId == companyId);
            taskEntityDTO.CompanyName = company.CompanyName;

            taskEntityDTO.AssignedDate = DateTime.Now;

            return View(taskEntityDTO);
        }


        //Create Task - POST
        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_SuperAdmin)]
        public async Task<IActionResult> CreateTask(TaskEntityDTO taskEntity)
        {
            return null;
        }


    }
}