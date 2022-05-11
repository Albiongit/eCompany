using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCompany.Models.DTOs.Entities
{
    public class CompanyDTO
    {
        public int CompanyId { get; set; }

        public string CompanyName { get; set; }

        public string? CompanyPhone { get; set; }

        public string? CompanyState { get; set; }

        public string? CompanyWeb { get; set; }
    }
}
