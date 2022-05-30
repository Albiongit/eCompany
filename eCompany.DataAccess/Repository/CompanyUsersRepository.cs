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
    public class CompanyUsersRepository : Repository<Company_User>, ICompanyUsersRepository
    {
        private readonly ApplicationDbContext _db;

        public CompanyUsersRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<IQueryable<ApplicationUser>> GetAllUsers(int companyId)
        {
            var userCompanies =  _db.Companies_Users
                .Include(x => x.ApplicationUser)
                .Where(x => x.CompanyId == companyId)
                .Select(x => x.ApplicationUser)
                .AsQueryable();

            return userCompanies;
        }

        public async Task<ApplicationUserDTO> GetUserProfile(string userId)
        {
            var userFromDb = await _db.Companies_Users
                .Include(x => x.ApplicationUser)
                .Include(x => x.Company)
                .Where(x => x.UserId == userId)
                .Select(x => new ApplicationUserDTO
                {
                    Name = x.ApplicationUser.Name,
                    Sex = x.ApplicationUser.Sex,
                    City = x.ApplicationUser.City,
                    State = x.ApplicationUser.State,
                    ImageUrl = x.ApplicationUser.ImageUrl,
                    CompanyName = x.Company.CompanyName,
                    CompanyId = x.Company.CompanyId
                })
                .FirstOrDefaultAsync();

            


            return userFromDb;
        }
       
        
    }
}
