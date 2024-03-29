﻿using eCompany.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace eCompany.ViewComponents
{
    [ViewComponent(Name = "UserProfile")]
    public class UserProfileViewComponent : ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;

       
        public UserProfileViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var UserFromDb = await _unitOfWork.ApplicationUser.GetFirstOrDefaultAsync(x => x.Id == claim.Value);

            return View(UserFromDb);
        }
    }
}
