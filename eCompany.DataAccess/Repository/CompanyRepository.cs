using eCompany.DataAccess.Data;
using eCompany.DataAccess.Repository.IRepository;
using eCompany.Models;
using eCompany.Models.DTOs.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCompany.DataAccess.Repository
{
    public class CompanyRepository : Repository<Company>, ICompanyRepository
    {
        private readonly ApplicationDbContext _db;

        public CompanyRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<IQueryable<Company>> GetAllCompanies()
        {
            var companyList = _db.Companies.AsQueryable();

            return companyList;
        }

        public async Task<CompanyDTO?> GetCompany(int companyId)
        {
            var companyDetails = await _db.Companies
                        .Where(x => x.CompanyId == companyId)
                        .Select(x => new CompanyDTO
                        {
                            CompanyId = x.CompanyId,
                            CompanyName = x.CompanyName,
                            CompanyPhone = x.CompanyPhone,
                            CompanyState = x.CompanyState,
                            CompanyWeb = x.CompanyWeb
                        })
                        .FirstOrDefaultAsync();

            return companyDetails;
        }


        public void Update(Company company)
        {
             _db.Update(company);
        }
    }
}
