using EmpReManagement.Models;

namespace EmpReManagement.ViewModel
{
    public class DepartementEmpViewModel
    {
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }

        // A list of employees related to the department
        public List<Employee> Employees { get; set; }    

    }
}
