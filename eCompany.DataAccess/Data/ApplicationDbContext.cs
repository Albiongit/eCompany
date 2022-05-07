using eCompany.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCompany.DataAccess.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Company_User>()
                .HasOne(c => c.Company)
                .WithMany(cu => cu.Company_Users)
                .HasForeignKey(ci => ci.CompanyId);

            modelBuilder.Entity<Company_User>()
                .HasOne(u => u.ApplicationUser)
                .WithMany(cu => cu.Company_Users)
                .HasForeignKey(ui => ui.UserId);

            base.OnModelCreating(modelBuilder);
        }



        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Company_User> Companies_Users { get; set; }

        
    }
}
