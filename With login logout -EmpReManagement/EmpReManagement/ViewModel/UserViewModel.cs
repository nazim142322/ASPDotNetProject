using System.ComponentModel.DataAnnotations;

namespace EmpReManagement.ViewModel
{
    public class UserViewModel
    {
        [Key]
        public Guid id { get; set; }

        [DataType(DataType.PhoneNumber)]
        [Required]
        public string Name { get; set; }
        [Required(ErrorMessage = "The Phone No field is required.")]
        [Display(Name="Phone No")]

        public string PhoneNo { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
