 using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCompany.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string? State { get; set; }
        [Required]
        public string? City { get; set; }

        public string? ImageUrl { get; set; }
        [Required]
        public char Sex { get; set; }


        [NotMapped]
        [Required]
        public string Role { get; set; }

        public ICollection<TaskEntity> Tasks { get; set; }




        // Navigations Properties
        public List<Company_User> Company_Users { get; set; }
    }
}
