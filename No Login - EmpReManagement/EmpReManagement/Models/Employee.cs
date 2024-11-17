
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace EmpReManagement.Models
{
    public class Employee
    {
        [Key]
        public int EmployeeId { get; set; }       
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName => $"{FirstName} {LastName}";
        public DateOnly DateOfBirth { get; set; }
        public String Gender { get; set; }        
        public string Email { get; set; }      
        public string PhoneNumber { get; set; }        
        public string Address { get; set; }        
        public bool IsActive { get; set; }
        public string ImagePath { get; set; }

        //relation with Department
        [ForeignKey("Department")]
        public int DepartmentId { get; set; } //foreign key

        [JsonIgnore] // Prevents circular reference during serialization
        public Department Departments { get; set; }//reference navigation property
    }
}
