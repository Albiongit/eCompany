﻿using eCompany.DataAccess.Data;
using eCompany.DataAccess.Repository.IRepository;
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

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            Company = new CompanyRepository(_db);
            ApplicationUser = new ApplicationUserRepository(_db);
            CompanyUsers = new CompanyUsersRepository(_db);
        }

        public ICompanyRepository Company{ get; private set; }

        public IApplicationUserRepository ApplicationUser { get; private set; }
        public ICompanyUsersRepository CompanyUsers { get; private set; }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}