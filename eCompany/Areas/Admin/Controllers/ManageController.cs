﻿using eCompany.DataAccess.Data;
using eCompany.DataAccess.Repository.IRepository;
using eCompany.Models;
using eCompany.Models.DTOs.Entities;
using eCompany.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;

namespace eCompany.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_SuperAdmin)]
    public class ManageController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _db;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IMapper _mapper;

        public ManageController(IUnitOfWork unitOfWork,
                                ApplicationDbContext db,
                                UserManager<IdentityUser> userManager,
                                RoleManager<IdentityRole> roleManager,
                                IWebHostEnvironment hostEnvironment,
                                IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
            _hostEnvironment = hostEnvironment;
            _mapper = mapper;
        }


        
        // GET - Index Manage
        [HttpGet]
        public async Task<IActionResult> Index(int? id)
        {
            var companyFromDb = await _unitOfWork.Company.GetFirstOrDefaultAsync(u => u.CompanyId == id);

            CompanyDTO company = new CompanyDTO
            {
                CompanyId = companyFromDb.CompanyId,
                CompanyName = companyFromDb.CompanyName,
                CompanyPhone = companyFromDb.CompanyPhone,
                CompanyState = companyFromDb.CompanyState,
                CompanyWeb = companyFromDb.CompanyWeb
            };

            return View(company);
        }



        [HttpGet]
        public async Task<IActionResult> GetUpdatePartial(int? id)
        {
            var companyFromDb = await _unitOfWork.Company.GetFirstOrDefaultAsync(u => u.CompanyId == id);

            CompanyDTO company = new CompanyDTO
            {
                CompanyId = companyFromDb.CompanyId,
                CompanyName = companyFromDb.CompanyName,
                CompanyPhone = companyFromDb.CompanyPhone,
                CompanyState = companyFromDb.CompanyState,
                CompanyWeb = companyFromDb.CompanyWeb
            };

            return PartialView("_UpdatePartial", company);
        }


        [HttpGet]
        [Authorize(Roles = SD.Role_SuperAdmin)]
        public async Task<IActionResult> GetEmployees(int? id, string? status)
        {
            var company = await _unitOfWork.Company.GetFirstOrDefaultAsync(uc => uc.CompanyId == id);
            CompanyDTO companyDTO = new CompanyDTO
            {
                CompanyId = company.CompanyId,
                CompanyName = company.CompanyName,
                CompanyPhone = company.CompanyPhone,
                CompanyState = company.CompanyState,
                CompanyWeb = company.CompanyWeb
            };

            return View(companyDTO);
        }



        //POST - Update Company
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = SD.Role_SuperAdmin)]
        public async Task<IActionResult> Update(CompanyDTO obj)
        {
            if (ModelState.IsValid)
            {
                var companyEntity = new Company
                {
                    CompanyId = obj.CompanyId,
                    CompanyName = obj.CompanyName,
                    CompanyPhone = obj.CompanyPhone,
                    CompanyState = obj.CompanyState,
                    CompanyWeb = obj.CompanyWeb
                };

                _unitOfWork.Company.Update(companyEntity);
                TempData["success"] = "Company updated successfully!";

                _unitOfWork.Save();

                return RedirectToAction("Index");
            }
            return View(obj);
        }




        //GET  - Update Employee
        [HttpGet]
        [Authorize(Roles = SD.Role_SuperAdmin)]
        public async Task<IActionResult> UpdateEmployee(string id)
        {

            var employee = await _unitOfWork.ApplicationUser.GetFirstOrDefaultAsync(u => u.Id == id);
            var userRoles = await _userManager.GetRolesAsync(employee);


            var userFromDb_eg = await _unitOfWork.CompanyUsers.GetUserProfile(employee.Id);
            var userProfileModel = _mapper.Map<ApplicationUserDTO>(userFromDb_eg);

            userProfileModel.Role = userRoles[0];
            userProfileModel.RoleList = _roleManager.Roles.Where(r => r.Name != SD.Role_SuperAdmin).Select(x => x.Name).Select(i => new SelectListItem
            {
                Text = i,
                Value = i
            });


            return View(userProfileModel);
        }


        //POST  -  Update Employee
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = SD.Role_SuperAdmin)]
        public async Task<IActionResult> UpdateEmployee(ApplicationUserDTO applicationUser, IFormFile? file = null)
        {
            var StatusMessage = "Nothing in the profile has changed";

            var userFromDb = _db.ApplicationUsers.FirstOrDefault(u => u.Id == applicationUser.Id);
            if (userFromDb == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                return RedirectToAction("UpdateEmployee", new { id = applicationUser.Id });
            }


            //name check
            if (applicationUser.Name != userFromDb.Name)
            {
                if (applicationUser.Name != null)
                {
                    userFromDb.Name = applicationUser.Name;
                    StatusMessage = "Your profile has been updated";

                }
                else
                {
                    StatusMessage = "Error, your profile has not been changed";
                }
            }

            //sex check
            if (applicationUser.Sex != userFromDb.Sex)
            {
                if (applicationUser.Sex == 'M' || applicationUser.Sex == 'F')
                {

                    if (!StatusMessage.Contains("Error"))
                    {
                        userFromDb.Sex = applicationUser.Sex;
                        StatusMessage = "Your profile has been updated";
                    }
                }
                else
                {
                    StatusMessage = "Error, your profile's sex gender has not been changed";
                }
            }

            //phone number check
            var phoneNumber = await _userManager.GetPhoneNumberAsync(userFromDb);
            if (applicationUser.PhoneNumber != phoneNumber)
            {

                if (applicationUser.PhoneNumber != null)
                {
                    if (!StatusMessage.Contains("Error"))
                    {
                        userFromDb.PhoneNumber = applicationUser.PhoneNumber;
                        StatusMessage = "Your profile has been updated";

                    }
                }
                else
                {
                    StatusMessage = "Error, your profile's phone number has not been changed";
                }
            }

            //state check
            if (applicationUser.State != userFromDb.State)
            {
                if (applicationUser.State != null)
                {

                    if (!StatusMessage.Contains("Error"))
                    {
                        userFromDb.State = applicationUser.State;
                        StatusMessage = "Your profile has been updated";
                    }
                }
                else
                {
                    StatusMessage = "Error, your profile's state name has not been changed";
                }
            }

            //city check
            if (applicationUser.City != userFromDb.City)
            {
                if (applicationUser.City != null)
                {

                    if (!StatusMessage.Contains("Error"))
                    {
                        userFromDb.City = applicationUser.City;
                        StatusMessage = "Your profile has been updated";
                    }
                }
                else
                {
                    StatusMessage = "Error, your profile's city name has not been changed";
                }
            }

            //image check
            string wwwRootPath = _hostEnvironment.WebRootPath;
            if (file != null)
            {
                string fileName = Guid.NewGuid().ToString();
                var uploads = Path.Combine(wwwRootPath, @"images\users");
                var extension = Path.GetExtension(file.FileName);

                using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                {
                    file.CopyTo(fileStreams);
                }
                userFromDb.ImageUrl = @"\images\users\" + fileName + extension;
                StatusMessage = "Your profile has been updated";
            }


            //saving changes
            _unitOfWork.Save();


            return RedirectToAction("UpdateEmployee", new { id = applicationUser.Id });

        }



        [HttpGet]
        [Authorize(Roles = SD.Role_SuperAdmin)]
        public async Task<ActionResult> GetTasks(int companyId)
        {
            var companyDetails = await _unitOfWork.Company.GetFirstOrDefaultAsync(c => c.CompanyId == companyId);
            if (companyDetails != null)
            {
                CompanyDTO company = new CompanyDTO
                {
                    CompanyId = companyDetails.CompanyId,
                    CompanyName = companyDetails.CompanyName,
                    CompanyPhone = companyDetails.CompanyPhone,
                    CompanyState = companyDetails.CompanyState,
                    CompanyWeb = companyDetails.CompanyWeb
                };
                return View(company);
            }

            return View();
        }







        #region API CALLS

        [HttpGet]
        [Authorize(Roles = SD.Role_SuperAdmin)]
        public async Task<IActionResult> GetAll(int? id)
        {
            var CompanyUsers = await _unitOfWork.CompanyUsers.GetFirstOrDefaultAsync(uc => uc.CompanyId == id);
            var CompanyId = CompanyUsers.CompanyId;

            var companyUsers = await _unitOfWork.CompanyUsers
                .GetAllUsers(CompanyId, "employee");

            return Json(new { data = companyUsers });
        }


        [HttpPost]
        [Authorize(Roles = SD.Role_SuperAdmin)]
        public async Task<JsonResult> GetEmployeeList(int id, string? status)
        {
           
            int totalRecord = 0;
            int filterRecord = 0;
            var draw = Request.Form["draw"].FirstOrDefault();
            var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
            var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
            var searchValue = Request.Form["search[value]"].FirstOrDefault();
            int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
            int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");

            var data = await _unitOfWork.CompanyUsers
                .GetAllUsers(id, status);

            if (status == "employees")
            {
                 data = await _unitOfWork.CompanyUsers
                    .GetAllUsers(id, "Employee");
            } else if(status == "admins")
            {
                 data = await _unitOfWork.CompanyUsers
                    .GetAllUsers(id, "Company Admin");
            }

            totalRecord = data.Count();

            if (!string.IsNullOrEmpty(searchValue))
            {
                data = data.Where(x => x.Name.ToLower().Contains(searchValue.ToLower()) || x.Email.ToLower().Contains(searchValue.ToLower()));
            }

            filterRecord = data.Count();

            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDirection))
            {
                data = data.OrderBy(sortColumn + " " + sortColumnDirection);
            }


            var empList = data.Skip(skip).Take(pageSize).ToList();

            var returnObj = new
            {
                draw = draw,
                recordsTotal = totalRecord,
                recordsFiltered = filterRecord,
                data = empList
            };

            return Json(returnObj);
        }


        // Delete company user 
        [HttpDelete]
        [Authorize(Roles = SD.Role_SuperAdmin)]
        public async Task<IActionResult> Delete(string? userId, int? id)
        {
            var userFromDb = await _unitOfWork.ApplicationUser.GetFirstOrDefaultAsync(x => x.Id == userId);
            if (userFromDb == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            await _unitOfWork.ApplicationUser.RemoveAsync(userFromDb);
            _unitOfWork.Save();

            return RedirectToAction("GetEmployees", new { id = id });
        }


        #endregion
    }
}
