using System.ComponentModel.DataAnnotations;

namespace EmpReManagement.Models
{
    public class Department
    {
        [Key]
        public int DepartmentId { get; set; }//primary key

        public string? Name { get; set; }

        //relation with employees
        public ICollection<Employee> Employees { get; set; } //= new List<Employee>();//collection navigation property
    }
}
