using AutoMapper;
using eCompany.DataAccess.Data;
using eCompany.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCompany.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext _db;
        private UserManager<IdentityUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;

        public UnitOfWork(ApplicationDbContext db, 
                          RoleManager<IdentityRole> roleManager,
                          UserManager<IdentityUser> userManager,
                          IMapper mapper)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
            Company = new CompanyRepository(_db);
            ApplicationUser = new ApplicationUserRepository(_db);
            CompanyUsers = new CompanyUsersRepository(_db, _roleManager, _userManager);
            Tasks = new TaskRepository(_db);
        }

        public ICompanyRepository Company{ get; private set; }

        public IApplicationUserRepository ApplicationUser { get; private set; }
        public ICompanyUsersRepository CompanyUsers { get; private set; }
        public ITaskRepository Tasks { get; private set; }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
