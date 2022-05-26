using eCompany.DataAccess.Data;
using eCompany.DataAccess.Repository.IRepository;
using eCompany.Models;
using eCompany.Models.DTOs.Entities;
using eCompany.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace eCompany.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ManageController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _db;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IWebHostEnvironment _hostEnvironment;

        public ManageController(IUnitOfWork unitOfWork,
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
        public async Task<IActionResult> GetEmployees(int? id)
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
        public async Task<IActionResult> UpdateEmployee(string id)
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
                CompanyId = userFromDb.CompanyId,
                RoleList = _roleManager.Roles.Where(r => r.Name != SD.Role_SuperAdmin).Select(x => x.Name).Select(i => new SelectListItem
                {
                    Text = i,
                    Value = i
                })

            };

            return View(userEmployee);
        }


        //POST  -  Update Employee
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateEmployee(ApplicationUserDTO applicationUser, IFormFile? file = null)
        {
            String StatusMessage = "Nothing in the profile has changed";

            var userFromDb = _db.ApplicationUsers.FirstOrDefault(u => u.Id == applicationUser.Id);
            if (userFromDb == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid) 
            {
                return RedirectToAction("UpdateEmployee", applicationUser.Id);
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







        #region API CALLS

        [HttpGet]
        public async Task<IActionResult> GetAll(int? id)
        {
            var CompanyUsers = await _unitOfWork.CompanyUsers.GetFirstOrDefaultAsync(uc => uc.CompanyId == id);
            var CompanyId = CompanyUsers.CompanyId;

            var companyUsers = await _unitOfWork.CompanyUsers
                .GetAllUsers(CompanyId);

            return Json(new { data = companyUsers });
        }


        [HttpDelete]
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
