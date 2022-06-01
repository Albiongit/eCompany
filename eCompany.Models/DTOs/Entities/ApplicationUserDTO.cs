using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCompany.Models.DTOs.Entities
{
    public class ApplicationUserDTO
    {
        public string? Id { get; set; }
        [Required]
        public string? Name { get; set; }
        
        public string? Email { get; set; }
        [Required]
        public string? State { get; set; }
        [Required]
        public string? PhoneNumber { get; set; }
        [Required]
        public string? City { get; set; }

        public string? ImageUrl { get; set; }
        [Required]
        public char Sex { get; set; }
        
        public string? Role { get; set; }
        
        public string? CompanyName { get; set; }
        public int? CompanyId { get; set; }

        public IEnumerable<SelectListItem>? RoleList { get; set; }

        public string? StatusMessage { get; set; } 

    }
}
