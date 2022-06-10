using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCompany.Models.DTOs.Entities
{
    public class TaskEntityDTO
    {
        public int? TaskId { get; set; }
        [Required]
        public string Title { get; set; }
        public string? Description { get; set; }
        public string? Comment { get; set; }
        public DateTime AssignedDate { get; set; }
        public DateTime? FinishedDate { get; set; }
        public DateTime? DueDate { get; set; }
        [Required]
        public int DayDuration { get; set; }
        public Status? Status { get; set; }
        public string EmployeeId { get; set; }
        
        public string? EmployeeName { get; set; }
        

        public int CompanyId { get; set; }

        public string? CompanyName { get; set; }
        public string? CompanyPhone { get; set; }
        
        public string? CompanyState { get; set; }

        public string? CompanyWeb { get; set; }
        public string? ErrorMessage { get; set; }
        public string? DueDateTask { get; set; }
        public string? StatusInfo { get; set; }

        public string? SuperAdminId { get; set; }

        public IEnumerable<SelectListItem>? EmployeeList { get; set; }
        public IEnumerable<SelectListItem>? StatusList { get; set; }
    }
}
