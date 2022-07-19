using eCompany.Models;
using eCompany.Models.DTOs.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCompany.DataAccess.Repository.IRepository
{
    public interface IApplicationUserRepository : IRepository<ApplicationUser>
    {
        // void Save(ApplicationUser user)

        public Task<ApplicationUserDTO> GetUserDetails(string email); 

    }
}
