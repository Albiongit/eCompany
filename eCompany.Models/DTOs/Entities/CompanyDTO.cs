using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCompany.Models.DTOs.Entities
{
    public class CompanyDTO
    {
        [Required]
        public int CompanyId { get; set; }
        [Required]
        [Display(Name = "Company Name")]
        public string CompanyName { get; set; }
        [Required]
        [Display(Name = "Company Phone Number")]
        public string? CompanyPhone { get; set; }
        [Required]
        [Display(Name = "Company State ")]
        public string? CompanyState { get; set; }
        [Required]
        [Display(Name = "Company WebSite")]
        public string? CompanyWeb { get; set; }

        public string? UserId { get; set; }
    }
}
