using eCompany.DataAccess.Data;
using eCompany.DataAccess.Repository.IRepository;
using eCompany.Models;
using eCompany.Models.DTOs.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Authorization;
using eCompany.Shared;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.SignalR.Client;
using AutoMapper;

namespace eCompany.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TaskController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _db;
        private readonly IEmailSender _emailSender;
        private readonly IMapper _mapper;

        private string errorMessage = "";

        public TaskController(IUnitOfWork unitOfWork, ApplicationDbContext db, IEmailSender emailSender, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _db = db;
            _emailSender = emailSender;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_SuperAdmin)]
        public async Task<IActionResult> Index()
        {

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);


            var companyFromDb = await _unitOfWork.CompanyUsers.GetCompanyDetails(claim.Value);

            var companyDetailsModel = _mapper.Map<CompanyDTO>(companyFromDb);

            if (companyDetailsModel != null)
            {
                return View(companyDetailsModel);
            }

            return View();
        }

        

        //Create Task - GET
        [HttpGet]
        [Authorize(Roles = SD.Role_SuperAdmin + "," + SD.Role_Admin)]
        public async Task<IActionResult> CreateTask(int companyId, string? id)
        {
            TaskEntityDTO taskEntityDTO = new TaskEntityDTO();
            taskEntityDTO.CompanyId = companyId;

            // Super Admin check
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier) ;


            var companyAdmin = await _unitOfWork.CompanyUsers.GetFirstOrDefaultAsync(x => x.UserId == claim.Value);

            if (companyAdmin == null)
            {
                //Then it's Super Admin
                taskEntityDTO.SuperAdminId = claim.Value;
            }

            if (id == null)
            {

                var getAllEmployees = await _unitOfWork.CompanyUsers.GetAllUsers(companyId, "Employee");

                taskEntityDTO.EmployeeList = getAllEmployees.Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                });
            }else
            {
                var getEmployee = await _unitOfWork.CompanyUsers.GetUserDetails(id);
                var employee = await _unitOfWork.ApplicationUser.GetFirstOrDefaultAsync(u => u.Id == id);
                taskEntityDTO.EmployeeName = employee.Name;
                taskEntityDTO.EmployeeId = id;

                taskEntityDTO.EmployeeList = getEmployee.Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString(),
                
            });


            }

            var company = await _unitOfWork.Company.GetFirstOrDefaultAsync(c => c.CompanyId == companyId);
            taskEntityDTO.CompanyName = company.CompanyName;

            taskEntityDTO.AssignedDate = DateTime.Now;

            return View(taskEntityDTO);
        }




        // Select for dropdown list of employees
        [HttpGet]
        public async Task<IActionResult> Search(int companyId, string term)
        {
            if (!string.IsNullOrEmpty(term))
            {
                var getEmployees = await _unitOfWork.CompanyUsers.GetAllUsers(companyId, "Employee");
                var data = getEmployees.Where(e => e.Name.Contains(term)).ToList().Take(5);
                return Ok(data);
            }
            else
            {
                var getEmployees = await _unitOfWork.CompanyUsers.GetAllUsers(companyId, "Employee");
                var data = getEmployees.ToList().Take(5);
                return Ok(data);
            }
        }




        //Create Task - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_SuperAdmin)]
        public async Task<IActionResult> CreateTask(TaskEntityDTO taskEntityDTO)
        {
            // Super Admin check
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var companyAdmin = await _unitOfWork.CompanyUsers.GetUserProfile(claim?.Value);
            

            if (ModelState.IsValid)
            {
                var taskEntity = new TaskEntity
                {
                    Title = taskEntityDTO.Title,
                    Description = taskEntityDTO.Description,
                    DayDuration = taskEntityDTO.DayDuration,
                    AssignedDate = taskEntityDTO.AssignedDate,
                    Status = Status.New,
                    EmployeeId = taskEntityDTO.EmployeeId,
                    CompanyID = taskEntityDTO.CompanyId,

                };

                await _unitOfWork.Tasks.AddAsync(taskEntity);
                TempData["success"] = "Task created successfully!";

                _unitOfWork.Save();

              

                var selectedEmployee = await _unitOfWork.ApplicationUser.GetFirstOrDefaultAsync(x => x.Id == taskEntityDTO.EmployeeId);
                string employeeEmail = selectedEmployee.Email;

                // send email for task confirmation

                

                if (companyAdmin != null)
                {
                    var adminModel = _mapper.Map<ApplicationUserDTO>(companyAdmin);

                    await _emailSender.SendEmailAsync(employeeEmail, "New Task - " + taskEntityDTO.CompanyName, "<p>New task assigned \" " + taskEntityDTO.Title + " \" for you, visit the webpage for the task details.</br>" +
                                                                                                         "Task assigned from " + adminModel.Name + " - "   + adminModel.CompanyName + " Company.</p>");
                    return RedirectToAction("Index", "Task");
                }

                // Otherwise if its Super Admin then
                await _emailSender.SendEmailAsync(employeeEmail, "New Task - " + taskEntityDTO.CompanyName, "<p>New task assigned \" " + taskEntityDTO.Title + " \" for you, visit the webpage for the task details.</br>" +
                                                                                                         "Task assigned from Super Admin - " /* + selectedEmployee.CompanyName*/ /*+ " Company.</p>"*/);
                return RedirectToAction("GetTasks", "Manage", new { companyId = taskEntityDTO.CompanyId });

            }

            return View(taskEntityDTO);
        }


        [HttpGet]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_SuperAdmin)]
        public async Task<IActionResult> Details(int taskId, string? errorMessage)
        {
            var taskEntity = await _unitOfWork.Tasks.GetTaskDetails(taskId);

            // Super Admin check
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);
            var companyAdmin = await _unitOfWork.CompanyUsers.GetFirstOrDefaultAsync(x => x.UserId == claim.Value);

            if(companyAdmin == null)
            {
                //So its Super Admin
                taskEntity.SuperAdminId = claim?.Value;
            }

            taskEntity.StatusList = new SelectList(Enum.GetNames(typeof(Status)));
            taskEntity.ErrorMessage = errorMessage;

            if (taskEntity != null)
            {
                return View(taskEntity);
            }

            return View();
        }


        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_SuperAdmin)]
        public async Task<IActionResult> Details(TaskEntityDTO taskEntityDTO)
        {
            if (ModelState.IsValid)
            {
                var taskFromDb = await _unitOfWork.Tasks.GetFirstOrDefaultAsync(t => t.TaskId == taskEntityDTO.TaskId);
                
                    
                    if(taskEntityDTO.FinishedDate != null)
                    {
                        if(taskEntityDTO.FinishedDate < DateTime.Now)
                        {
                            errorMessage = "Invalid date selected, please select a valid finished task date!";
                            return RedirectToAction("Details", new { taskId = taskEntityDTO.TaskId, errorMessage = errorMessage });  
                        }

                        taskFromDb.FinishedDate = taskEntityDTO.FinishedDate;
                        taskFromDb.Status = Status.Done;
                        taskFromDb.Title = taskEntityDTO.Title;
                        taskFromDb.DayDuration = taskEntityDTO.DayDuration;
                        taskFromDb.Description = taskEntityDTO.Description;
                    }
                    else
                    {
                        taskFromDb.Title = taskEntityDTO.Title;
                        taskFromDb.DayDuration = taskEntityDTO.DayDuration;
                        taskFromDb.Description = taskEntityDTO.Description;
                        taskFromDb.Status = taskEntityDTO.Status ??= taskFromDb.Status;

                    }

                _unitOfWork.Tasks.Update(taskFromDb);
                _unitOfWork.Save();
                
                return RedirectToAction("Details", "Task", new { taskId = taskEntityDTO.TaskId });
                
            }

            errorMessage = "Invalid credentials, please fill out the form correctly.";
            return RedirectToAction("Details", new { taskId = taskEntityDTO.TaskId ,errorText = errorMessage });
        }

        [HttpGet]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_SuperAdmin)]
        public async Task<ActionResult> Activity(string id)
        {
            
            var employee = await _unitOfWork.CompanyUsers.GetUserProfile(id);

            var employeeModel = _mapper.Map<ApplicationUserDTO>(employee);

            if(employeeModel != null)
            {
                return View(employeeModel);
            }

            return View();
        }

        [HttpGet]
        public async Task<JsonResult> GetTasksChart(string id)
        {
            var employeeTasks = await _unitOfWork.Tasks.GetEmployeeTasks(id, null);
            var stats = employeeTasks.Select(x => x.Status).Distinct();

            string[] statsName = new string[] { "New", "Active", "Done", "Problem", "Rejected" };
            Chart _chart = new Chart();
            _chart.labels = statsName.ToArray();
            _chart.datasets = new List<Datasets>();



            for (int i = 0; i < 5; i++)
            {
                int j = i * 40;
                int red = 50 + j;
                int green = 50 + j;
                int blue = 10 + j;
                _chart.datasets.Add(new Datasets()
                {
                    label = _chart.labels[i],
                    data = employeeTasks.Count(x => x.Status == (Status)i),
                    backgroundColor = "rgba("+red+","+green+","+blue+",.5)",
                    borderColor = "rgb(" + red + "," + green + "," + blue + ")",
                    borderWidth = 1
                });
            }

            return Json(_chart);
        }


        [HttpGet]
        public async Task<JsonResult> GetMonthlyTasks(string id)
        {
            var employeeTasks = await _unitOfWork.Tasks.GetEmployeeMonthlyTasks(id);

            string[] statsName = new string[] { "New", "Active", "Done", "Problem", "Rejected" };
            Chart _chart = new Chart();
            _chart.labels = statsName.ToArray();
            _chart.datasets = new List<Datasets>();



            for (int i = 0; i < 5; i++)
            {
                int j = i * 40;
                int red = 50 + j;
                int green = 10 + j;
                int blue = 50 + j;
                _chart.datasets.Add(new Datasets()
                {
                    label = _chart.labels[i],
                    data = employeeTasks.Count(x => x.Status == (Status)i),
                    backgroundColor = "rgba(" + red + "," + green + "," + blue + ",.5)",
                    borderColor = "rgb(" + red + "," + green + "," + blue + ")",
                    borderWidth = 1
                });
            }

            return Json(_chart);
        }





        #region APICALLS

        [HttpPost]
        public async Task<JsonResult> GetTaskList(int id, Status? status)
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
                .GetAllTasks(id, status);                  

            totalRecord = data.Count();

            if (!string.IsNullOrEmpty(searchValue))
            {
                data = data.Where(x =>  x.Title.ToLower().Contains(searchValue.ToLower()));
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

        

        [HttpPost]
        public async Task<JsonResult> GetUserTasks(string id, Status? status)
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
                data = data.Where(x => x.Title.ToLower().Contains(searchValue.ToLower()));
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



        [HttpDelete]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_SuperAdmin)]
        public async Task<IActionResult> Delete(int taskId, string? id)
        {
            var taskEntity = await _unitOfWork.Tasks.GetFirstOrDefaultAsync(t => t.TaskId == taskId);

            if(taskEntity == null)
            {
                return Json(new { success = false, message = "Error while deleting." });
            }

            await _unitOfWork.Tasks.RemoveAsync(taskEntity);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Task deleted successfully!" });

        }


        #endregion


    }
}
