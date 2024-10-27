using EmpReManagement.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace EmpReManagement.ViewModel
{
    public class DepartmentViewModel
    {
    
        // No need for [Key] in the ViewModel
        public int DepartmentId { get; set; }//primary key

        [Display(Name = "Department Name")]
        [Required]
        [StringLength(50, ErrorMessage = "Department name cannot be longer than 50 characters.")]
        //[MaxLength(50, ErrorMessage = "Department name cannot be longer than 50 characters.")]
        public string Name { get; set; }

        //relation with employees
        // Exclude this property from validation
        [ValidateNever]
        public ICollection<Employee> Employees { get; set; }        
    }
}
