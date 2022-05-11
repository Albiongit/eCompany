using eCompany.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace eCompany.ViewComponents
{
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

            var UserFromDb = _unitOfWork.ApplicationUser.GetFirstOrDefault(x => x.Id == claim.Value);

            return View(UserFromDb);
        }
    }
}
