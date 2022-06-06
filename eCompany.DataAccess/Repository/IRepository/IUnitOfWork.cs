using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCompany.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        ICompanyRepository Company{ get; }
        IApplicationUserRepository ApplicationUser{ get; }

        ICompanyUsersRepository CompanyUsers{ get; }
        ITaskRepository Tasks{ get; }


        void Save();
    }
}
