﻿using eCompany.Models;
using eCompany.Models.DTOs.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCompany.DataAccess.Repository.IRepository
{
    public interface ICompanyRepository : IRepository<Company>
    {
        void Update(Company company);

        public Task<IQueryable<Company>> GetAllCompanies();
        public Task<CompanyDTO?> GetCompany(int companyId);

    }
}
