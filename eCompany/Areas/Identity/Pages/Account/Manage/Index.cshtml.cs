// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using AutoMapper;
using eCompany.DataAccess.Data;
using eCompany.DataAccess.Repository.IRepository;
using eCompany.Models.DTOs.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace eCompany.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IMapper _mapper;

        public IndexModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IUnitOfWork unitOfWork,
            ApplicationDbContext db,
            IWebHostEnvironment hostEnvironment,
            IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _unitOfWork = unitOfWork;
            _db = db;
            _hostEnvironment = hostEnvironment; 
            _mapper = mapper;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        
        [Display(Name="Email")]
        public string Username { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        /// 

        public string Name { get; set; }
        public string Id { get; set; }
        public char Sex { get; set; }
        public string State { get; set; }

        [Display(Name="Profile Picture")]
        public string ImageUrl { get; set; }
        public string City { get; set; }
        public string Role { get; set; }

        [Display(Name = "Company Name")]
        public string CompanyName { get; set; }


        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }


        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }
        }

        private async Task LoadAsync(IdentityUser user)
        {
            //userName = email 
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            //var userFromDb = await _unitOfWork.ApplicationUser.GetFirstOrDefaultAsync(u => u.Id == user.Id);
            //var userRole =  _db.UserRoles.FirstOrDefault(x => x.UserId == user.Id);
            var userRoles = await _userManager.GetRolesAsync(user);

            if(userRoles[0] == "Super Admin")
            {
                var userFromDb = await _unitOfWork.ApplicationUser.GetFirstOrDefaultAsync(u => u.Id == user.Id);
                if(userFromDb != null)
                {
                    Id = userFromDb.Id;
                    Username = userFromDb.UserName;
                    Name = userFromDb.Name;
                    Sex = userFromDb.Sex;
                    City = userFromDb.City;
                    State = userFromDb.State;
                    Role = userRoles[0];
                    ImageUrl = userFromDb.ImageUrl;
                    PhoneNumber = userFromDb.PhoneNumber;
                }
            }
            else 
            {
                var userFromDb = await _unitOfWork.CompanyUsers.GetUserProfile(user.Id);
                var userDbModel =  _mapper.Map<ApplicationUserDTO>(userFromDb);
                userDbModel.Role = userRoles[0];
                //string userRoli = userRoles[0];

                Id = userDbModel.Id;
                Username = userName;
                Name = userDbModel.Name;
                Sex = userDbModel.Sex;
                City = userDbModel.City;
                State = userDbModel.State;
                Role = userDbModel.Role;
                CompanyName = userDbModel.CompanyName;
                ImageUrl = userDbModel.ImageUrl;
                PhoneNumber = userDbModel.PhoneNumber;

            }



            Input = new InputModel
            {
                PhoneNumber = phoneNumber
               
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(ApplicationUserDTO applicationUser, IFormFile? file = null)
        {

            StatusMessage = "Nothing in your profile has changed";

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var userFromDb =  _db.ApplicationUsers.FirstOrDefault(u => u.Id == user.Id);
            
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
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            Input.PhoneNumber = applicationUser.PhoneNumber;
            if (Input.PhoneNumber != phoneNumber)
            {
              
                if (Input.PhoneNumber != null)
                {
                    if (!StatusMessage.Contains("Error"))
                    {
                        var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
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


            await _signInManager.RefreshSignInAsync(user);
            return RedirectToPage();
        }
    }
}
