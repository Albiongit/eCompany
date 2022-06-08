using eCompany.DataAccess.Data;
using eCompany.DataAccess.Repository.IRepository;
using eCompany.Models;
using eCompany.Models.DTOs.Entities;
using eCompany.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace eCompany.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class TaskController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _db;

        private string errorMessage = "";


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
            
            if (companyFromDb != null)
            {
                var companyView = new CompanyDTO
                {
                    CompanyId = companyFromDb.CompanyId,
                    CompanyName = companyFromDb.CompanyName,
                    CompanyPhone = companyFromDb.CompanyPhone,
                    CompanyState = companyFromDb.CompanyState,
                    CompanyWeb = companyFromDb.CompanyWeb,
                    UserId = claim.Value
                };

                return View(companyView);
            }

            return View();
        }





        [HttpGet]
        [Authorize(Roles = SD.Role_Employee)]
        public async Task<IActionResult> Details(int taskId, string? errorMessage)
        {
            var taskEntity = await _unitOfWork.Tasks.GetTaskDetails(taskId);


            taskEntity.StatusList = new SelectList(Enum.GetNames(typeof(Status)));
            taskEntity.ErrorMessage = errorMessage;

            if (taskEntity != null)
            {
                return View(taskEntity);
            }

            return View();
        }


        [HttpPost]
        [Authorize(Roles = SD.Role_Employee)]
        public async Task<IActionResult> Details(TaskEntityDTO taskEntityDTO)
        {
            if (ModelState.IsValid)
            {
                var taskFromDb = await _unitOfWork.Tasks.GetFirstOrDefaultAsync(t => t.TaskId == taskEntityDTO.TaskId);


                if (taskEntityDTO.FinishedDate != null)
                {
                    if (taskEntityDTO.FinishedDate < DateTime.Now)
                    {
                        errorMessage = "Invalid date selected, please select a valid finished task date!";
                        return RedirectToAction("Details", new { taskId = taskEntityDTO.TaskId, errorMessage = errorMessage });
                    }

                    taskFromDb.FinishedDate = taskEntityDTO.FinishedDate;
                    taskFromDb.Status = Status.Done;
                    taskFromDb.Title = taskEntityDTO.Title;
                    taskFromDb.DayDuration = taskEntityDTO.DayDuration;
                    taskFromDb.Description = taskEntityDTO.Description;
                    taskFromDb.Comment = taskEntityDTO.Comment;
                }
                else
                {
                    taskFromDb.Title = taskEntityDTO.Title;
                    taskFromDb.DayDuration = taskEntityDTO.DayDuration;
                    taskFromDb.Description = taskEntityDTO.Description;
                    taskFromDb.Comment = taskEntityDTO.Comment;
                    taskFromDb.Status = taskEntityDTO.Status ??= taskFromDb.Status;

                }

                _unitOfWork.Tasks.Update(taskFromDb);
                _unitOfWork.Save();

                return RedirectToAction("Details", "Task", new { taskId = taskEntityDTO.TaskId });

            }

            errorMessage = "Invalid credentials, please fill out the form correctly.";
            return RedirectToAction("Details", new { taskId = taskEntityDTO.TaskId, errorText = errorMessage });
        }











        #region APICALLS

        [HttpPost]
        public async Task<JsonResult> GetTaskList(string id, Status? status)
        {

            int totalRecord = 0;
            int filterRecord = 0;
            var draw = Request.Form["draw"].FirstOrDefault();
            var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
            var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
            var searchValue = Request.Form["search[value]"].FirstOrDefault();
            int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
            int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");
            var data = await _unitOfWork.Tasks
                .GetEmployeeTasks(id, status);

            totalRecord = data.Count();

            if (!string.IsNullOrEmpty(searchValue))
            {
                data = data.Where(x => x.TaskId == Int32.Parse(searchValue) ||
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


        #endregion

    }
}
