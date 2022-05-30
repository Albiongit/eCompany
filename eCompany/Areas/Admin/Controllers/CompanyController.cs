using eCompany.DataAccess.Repository.IRepository;
using eCompany.Models;
using eCompany.Models.DTOs.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Dynamic.Core;

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

        

        //GET - Insert
        public async Task<IActionResult> Insert()
        {
            CompanyDTO company = new();

            //create company
            return View(company);
         
        }

        //POST - Insert
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Insert(CompanyDTO obj)
        {
            if (ModelState.IsValid)
            {
                var companyEntity = new Company
                {
                    CompanyName = obj.CompanyName,
                    CompanyPhone = obj.CompanyPhone,
                    CompanyState = obj.CompanyState,
                    CompanyWeb = obj.CompanyWeb
                };

                    await _unitOfWork.Company.AddAsync(companyEntity);
                    TempData["success"] = "Company created successfully!";
                
                _unitOfWork.Save();

                return RedirectToAction("Index", "Company");
            }
            return View(obj);
        }




        //POST - Update
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

                return RedirectToAction("Index", "Company");
            }
            return View(obj);
        }




        #region API CALLS
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var companyList = await _unitOfWork.Company.GetAllAsync();
            return Json(new { data = companyList });
        }

        [HttpPost]
        public async Task<JsonResult> GetCompanyList()
        {

            int totalRecord = 0;
            int filterRecord = 0;
            var draw = Request.Form["draw"].FirstOrDefault();
            var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
            var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
            var searchValue = Request.Form["search[value]"].FirstOrDefault();
            int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
            int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");
            var data = await _unitOfWork.Company.GetAllCompanies();

            totalRecord = data.Count();

            if (!string.IsNullOrEmpty(searchValue))
            {
                data = data.Where(x => x.CompanyName.ToLower().Contains(searchValue.ToLower()) || x.CompanyWeb.ToLower().Contains(searchValue.ToLower()));
            }

            filterRecord = data.Count();

            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDirection))
            {
                data = data.OrderBy(sortColumn + " " + sortColumnDirection);
            }


            var companyList = data.Skip(skip).Take(pageSize).ToList();

            var returnObj = new
            {
                draw = draw,
                recordsTotal = totalRecord,
                recordsFiltered = filterRecord,
                data = companyList
            };

            return Json(returnObj);
        }

        //POST
        [HttpDelete]
        public async Task<IActionResult> Delete(int? id)
        {
            var obj = await _unitOfWork.Company.GetFirstOrDefaultAsync(u => u.CompanyId == id);
            if(obj == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            await _unitOfWork.Company.RemoveAsync(obj);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Delete Successful" });
        }


        #endregion
    }
}
