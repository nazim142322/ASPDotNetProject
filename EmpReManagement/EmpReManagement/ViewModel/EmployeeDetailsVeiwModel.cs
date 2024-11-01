namespace EmpReManagement.ViewModel
{
    public class EmployeeDetailsVeiwModel
    {
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
        
        // New property for Department Name
        public string DepartmentName { get; set; }

    }
}
