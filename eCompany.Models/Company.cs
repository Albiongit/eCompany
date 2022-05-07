using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCompany.Models
{
    public class Company
    {
        [Key]
        public int CompanyId { get; set; }
    
        [Required]
        public string CompanyName { get; set; }

        public string? CompanyPhone { get; set; }

        public string? CompanyState { get; set; }

        public string? CompanyWeb { get; set; }



        // Navigations Properties
        public List<Company_User> Company_Users { get; set; }

    }
}
