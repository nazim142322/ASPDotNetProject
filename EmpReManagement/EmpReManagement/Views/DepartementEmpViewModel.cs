using EmpReManagement.Models;

namespace EmpReManagement.Views
{
    public class DepartementEmpViewModel
    {
        public int DepartmentId { get; set; } // Primary key

       public string DepartmentName { get; set; }

        // A list of employees related to the department
        public List<Employee> Employees { get; set; }
    }
}
