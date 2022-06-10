using eCompany.Models;
using eCompany.Models.DTOs.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCompany.DataAccess.Repository.IRepository
{
    public interface ICompanyUsersRepository : IRepository<Company_User>
    {
        public Task<IQueryable<ApplicationUserDTO>> GetAllUsers(int companyId, string? role);
        public Task<IQueryable<ApplicationUserDTO>> GetUserDetails(string id);

        public Task<ApplicationUserDTO> GetUserProfile(string id);
        public Task<CompanyDTO> GetCompanyDetails(string id);
        

    }
}
