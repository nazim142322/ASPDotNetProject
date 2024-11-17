using AutoMapper;
using EmpReManagement.Models;
using EmpReManagement.ViewModel;

namespace EmpReManagement.MapperProfiler
{
    public class YourMappingProfile:Profile
    {
        public YourMappingProfile()
        {
            CreateMap<Employee, EmployeeViewModel>().ReverseMap();
            //CreateMap<Employee, EmployeeViewModel>();
            //CreateMap<EmployeeViewModel, Employee>();

            //CreateMap<Employee, EmployeeDetailsVeiwModel>();



        }

    }
}
