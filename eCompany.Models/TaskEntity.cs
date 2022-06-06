using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCompany.Models
{
    public class TaskEntity
    {
        [Key]
        public int TaskId { get; set; }
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime AssignedDate { get; set; }
        public DateTime? FinishedDate { get; set; }
        public int DayDuration { get; set; }
        public Status Status { get; set; }
        public string EmployeeId { get; set; }
        public int CompanyID { get; set; }
        public ApplicationUser Employee { get; set; }
        public Company Company { get; set; }

    }
}
