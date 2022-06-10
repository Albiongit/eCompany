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
    public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
    {
        private readonly ApplicationDbContext _db;

        public ApplicationUserRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<ApplicationUserDTO> GetUserDetails(string email)
        {
            var getUser = (from aU in _db.ApplicationUsers
                           join uR in _db.UserRoles on aU.Id equals uR.UserId
                           join r in _db.Roles on uR.RoleId equals r.Id
                           where aU.UserName == email
                           select new ApplicationUserDTO
                           {
                               Id = aU.Id,
                               Name = aU.Name,
                               Email = aU.Email,
                               PhoneNumber = aU.PhoneNumber,
                               Sex = aU.Sex,
                               City = aU.City,
                               State = aU.State,
                               Role = r.Name
                           }
                            ).FirstOrDefault();

            return getUser;
        }
    }
}
