using eCompany.DataAccess.Repository.IRepository;
using eCompany.Models;
using eCompany.Models.DTOs.Entities;
using Microsoft.AspNetCore.Mvc;

namespace eCompany.Areas.Admin.Controllers
{
    [Area("Admin")]

    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
       
        public IActionResult Index()
        {
            return View();
        }

        //GET
        public IActionResult Upsert(int? id)
        {
            Company company = new();

            if(id == null || id == 0)
            {
                //create company
                return View(company);
            }
            else
            {
                company = _unitOfWork.Company.GetFirstOrDefault(u => u.CompanyId == id);
                return View(company);
            }
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(CompanyDTO obj)
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
                if (obj.CompanyId == 0)
                {
                    
                    _unitOfWork.Company.Add(companyEntity);
                    TempData["success"] = "Company created successfully!";
                }
                else
                {
                    _unitOfWork.Company.Update(companyEntity);
                    TempData["success"] = "Company updated successfully!";
                }
                _unitOfWork.Save();

                return RedirectToAction("Index");
            }
            return View(obj);
        }


        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            var companyList = _unitOfWork.Company.GetAll();
            return Json(new { data = companyList });
        }

        //POST
        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var obj = _unitOfWork.Company.GetFirstOrDefault(u => u.CompanyId == id);
            if(obj == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            _unitOfWork.Company.Remove(obj);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Delete Successful" });
        }


        #endregion
    }
}
