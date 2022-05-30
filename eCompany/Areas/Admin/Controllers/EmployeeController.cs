using eCompany.DataAccess.Data;
using eCompany.DataAccess.Repository.IRepository;
using eCompany.Models;
using eCompany.Models.DTOs.Entities;
using eCompany.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using System.Linq.Dynamic.Core;
using static eCompany.Areas.Identity.Pages.Account.LoginModel;
using System.Linq;

namespace eCompany.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class EmployeeController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _db;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IWebHostEnvironment _hostEnvironment;
        [TempData]
        public string? StatusMessage { get; set; }

        public EmployeeController(IUnitOfWork unitOfWork, 
                                  ApplicationDbContext db,  
                                  UserManager<IdentityUser> userManager, 
                                  RoleManager<IdentityRole> roleManager,
                                  IWebHostEnvironment hostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
            _hostEnvironment = hostEnvironment;
        }
        public async Task<IActionResult> Index()
        {
             var claimsIdentity = (ClaimsIdentity)User.Identity;
             var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

             var CompanyUsers = await _unitOfWork.CompanyUsers.GetFirstOrDefaultAsync(uc => uc.UserId == claim.Value);

             var companyDetails = await _unitOfWork.Company.GetFirstOrDefaultAsync(c => c.CompanyId == CompanyUsers.CompanyId);

             return View(companyDetails);
        }

        //GET
        public async Task<IActionResult> Update(string id)
        {   

            var employee = await _unitOfWork.ApplicationUser.GetFirstOrDefaultAsync(u => u.Id == id);
            var userRoles = await _userManager.GetRolesAsync(employee);

            var userName = await _userManager.GetUserNameAsync(employee);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(employee);



            var userFromDb = await _unitOfWork.CompanyUsers.GetUserProfile(employee.Id);
            userFromDb.Role = userRoles[0];



            ApplicationUserDTO userEmployee = new ApplicationUserDTO
            {
                        Id = id,
                        Name = userFromDb.Name,
                        City = userFromDb.City,
                        State = userFromDb.State,
                        Email = userName,
                        ImageUrl = userFromDb.ImageUrl,
                        Sex = userFromDb.Sex,
                        PhoneNumber = phoneNumber,
                        Role = userFromDb.Role,
                        CompanyName = userFromDb.CompanyName,
                        RoleList = _roleManager.Roles.Where(r => r.Name != SD.Role_SuperAdmin).Select(x => x.Name).Select(i => new SelectListItem
                        {
                            Text = i,
                            Value = i
                        })

                };
            

            


            return View(userEmployee);
        }


        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(ApplicationUserDTO applicationUser, IFormFile? file = null)
        {
            StatusMessage = "Nothing in the profile has changed";

            var userFromDb =  _db.ApplicationUsers.FirstOrDefault(u => u.Id == applicationUser.Id);
            if (userFromDb == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                return RedirectToAction("Update", applicationUser.Id);
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


            return RedirectToAction("Update", new { id = applicationUser.Id});

        }



        #region API CALLS

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var CompanyUsers = await _unitOfWork.CompanyUsers.GetFirstOrDefaultAsync(uc => uc.UserId == claim.Value);
            var CompanyId = CompanyUsers.CompanyId;

            var companyUsers = await _unitOfWork.CompanyUsers
                .GetAllUsers(CompanyId);

            return Json( new { data = companyUsers });
        }



        [HttpPost]
        public async Task<JsonResult> GetEmployeeList()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var CompanyUsers = await _unitOfWork.CompanyUsers.GetFirstOrDefaultAsync(uc => uc.UserId == claim.Value);
            var CompanyId = CompanyUsers.CompanyId;

            int totalRecord = 0;
            int filterRecord = 0;
            var draw = Request.Form["draw"].FirstOrDefault();
            var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
            var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
            var searchValue = Request.Form["search[value]"].FirstOrDefault();
            int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
            int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");
            var data = await _unitOfWork.CompanyUsers
                .GetAllUsers(CompanyId);

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




        [HttpDelete]
        public async Task<IActionResult> Delete(string? userId)
        {
            var userFromDb = await _unitOfWork.ApplicationUser.GetFirstOrDefaultAsync(x => x.Id == userId);
            if (userFromDb == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            
            await _unitOfWork.ApplicationUser.RemoveAsync(userFromDb);
            _unitOfWork.Save();
            return Json(new { success = false, message = "Delete Successful" });
        }


        #endregion

    }

    

}
