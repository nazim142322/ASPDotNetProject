using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Security.Policy;
using Humanizer;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace EmpReManagement.ViewModel
{
    public class EmployeeViewModel
    {
        public int EmployeeId { get; set; }

        [Display(Name = "First Name")]
        [Required(ErrorMessage = "First Name is required")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "Last Name is required")]
        public string LastName { get; set; }

        public string FullName => $"{FirstName} {LastName}";

        [Display(Name = "Date of Birth")]
        [Required(ErrorMessage = "Date of Birth is required")]
        [DataType(DataType.Date)]        
        public DateOnly DateOfBirth { get; set; }

        [Required(ErrorMessage = "Gender is required")]
        public string Gender { get; set; }

        [Display(Name = "Email Address")]
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone number is required")]        
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Phone No")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Phone number must contain only numbers")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Address is required")]
        public string Address { get; set; }

        [Display(Name = "Mark as Active")]
        [Required(ErrorMessage = "Please specify if the employee is active")]
        public bool IsActive { get; set; }

        // Foreign key for Department
        [ForeignKey("Department")]
        [Display(Name = "Department")]
        [Required(ErrorMessage = "Department is required")]        
        public int DepartmentId { get; set; } // Dropdown for departments

        //for Image
        //[Required(ErrorMessage = "Select Image")]
        [ValidateNever]        
        public IFormFile Photo { get; set; }

        //to map from  domain model employee to viewModel EmployeeViewModel



        [ValidateNever]
        public string ImagePath { get; set; }

        [ValidateNever]
        public List<SelectListItem> Departments { get; set; } = null!;//Populated list for dropdown, no validation needed here
    }
}
