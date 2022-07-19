using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace eCompany.Controllers
{
    [Area("Customer")]
    public class LanguageController : Controller
    {
        public async Task<IActionResult> Change(string culture)
        {
            Response.Cookies.Append(CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddMonths(1) }
                );

            return RedirectToAction("Index", "Home", new { area = "Customer" });
        }
    }
}
