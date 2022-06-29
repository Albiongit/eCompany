using AutoMapper;
using eCompany.DataAccess.Data;
using eCompany.DataAccess.Repository.IRepository;
using eCompany.Models;
using eCompany.Models.DTOs.Entities;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public CompanyUsersRepository(ApplicationDbContext db,
                                      RoleManager<IdentityRole> roleManager,
                                      UserManager<IdentityUser> userManager) : base(db)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IQueryable<ApplicationUserDTO>> GetAllUsers(int companyId, string? role)
        {

            if(role != null)
            {
                var query = (
                             from cU in _db.Companies_Users
                             join aU in _db.ApplicationUsers on cU.UserId equals aU.Id
                             join uR in _db.UserRoles on aU.Id equals uR.UserId
                             join r in _db.Roles on uR.RoleId equals r.Id
                             join c in _db.Companies on cU.CompanyId equals c.CompanyId
                             where r.Name == role
                             where cU.CompanyId == companyId
                             select new ApplicationUserDTO
                             {
                                 Id = aU.Id,
                                 Name = aU.Name,
                                 Email = aU.Email,
                                 PhoneNumber = aU.PhoneNumber,
                                 Sex = aU.Sex,
                                 City = aU.City,
                                 State = aU.State,
                                 ImageUrl = aU.ImageUrl,
                                 CompanyName = c.CompanyName,
                                 CompanyId = c.CompanyId,
                                 Role = r.Name
                             });

                return query;
            }
            else
            {
                var query = (from cU in _db.Companies_Users
                             join aU in _db.ApplicationUsers on cU.UserId equals aU.Id
                             join uR in _db.UserRoles on aU.Id equals uR.UserId
                             join r in _db.Roles on uR.RoleId equals r.Id
                             join c in _db.Companies on cU.CompanyId equals c.CompanyId
                             where cU.CompanyId == companyId
                             select new ApplicationUserDTO
                             {
                                 Id = aU.Id,
                                 Name = aU.Name,
                                 Email = aU.Email,
                                 PhoneNumber = aU.PhoneNumber,
                                 Sex = aU.Sex,
                                 City = aU.City,
                                 State = aU.State,
                                 ImageUrl = aU.ImageUrl,
                                 CompanyName = c.CompanyName,
                                 CompanyId = c.CompanyId,
                                 Role = r.Name
                             });

                return query;
            }

            

        }

        public async Task<Company> GetCompanyDetails(string id)
        {
            var companyDetails = await _db.Companies_Users
                    .Include(x => x.Company)
                    .Where(x => x.UserId == id)
                    .Select(x => x.Company)
                    .FirstOrDefaultAsync();


            return companyDetails;
        }


        public async Task<IQueryable<ApplicationUserDTO>> GetUserDetails(string userId)
        {
            var userFromDb =  _db.Companies_Users
                .Include(x => x.ApplicationUser)
                .Include(x => x.Company)
                .Where(x => x.UserId == userId)
                .Select(x => new ApplicationUserDTO
                {
                    Id = x.ApplicationUser.Id,
                    Name = x.ApplicationUser.Name,
                    Sex = x.ApplicationUser.Sex,
                    City = x.ApplicationUser.City,
                    State = x.ApplicationUser.State,
                    ImageUrl = x.ApplicationUser.ImageUrl,
                    CompanyName = x.Company.CompanyName,
                    CompanyId = x.Company.CompanyId,
                    PhoneNumber = x.ApplicationUser.PhoneNumber
                })
                .AsQueryable();




            return userFromDb;
        }

        public async Task<Company_User?> GetUserProfile(string id)
        {
            var userFromDb = await _db.Companies_Users
                    .Include(x => x.ApplicationUser)
                    .Include(x => x.Company)
                    .Where(x => x.UserId == id)
                    .FirstOrDefaultAsync();

            return userFromDb;
        }
    }
}
