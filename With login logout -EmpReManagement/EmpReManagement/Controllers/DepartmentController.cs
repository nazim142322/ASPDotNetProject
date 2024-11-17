using ClosedXML.Excel;
using EmpReManagement.Data;
using EmpReManagement.Models;
using EmpReManagement.ViewModel;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;

namespace EmpReManagement.Controllers
{
    public class DepartmentController : Controller
    {
        private readonly AppDbContext dbContext;

        public DepartmentController(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        //to show all departments
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> Index()
        {
            if(string.IsNullOrEmpty(HttpContext.Session.GetString("UserSession")))
            {
                return RedirectToAction("Login", "UserLoginRegistration");
            }
            //var departments = await dbContext.Departments.OrderBy(d=>d.Name).ToListAsync();
            IQueryable<Department> depts = dbContext.Departments.OrderBy(d => d.Name);
            var departments = await depts.ToListAsync(); // Execute query at this point

            // Create a list to hold DepartmentViewModel objects
            var departmentViewModels = new List<DepartmentViewModel>();//an empty list of DepartmentViewModel objects

            // Use foreach to manually map each Department model to DepartmentViewModel
            foreach (var department in departments)
            {
                var departmentViewModel = new DepartmentViewModel
                {
                    DepartmentId = department.DepartmentId,
                    Name = department.Name
                };

                // Add the mapped DepartmentViewModel to the list
                departmentViewModels.Add(departmentViewModel);
            }
            // Pass the list of DepartmentViewModel to the view
            return View(departmentViewModels);
        }

        //add Department UI
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public IActionResult Add()
        {
            if(string.IsNullOrEmpty(HttpContext.Session.GetString("UserSession")))
            {
                return RedirectToAction("Login", "UserLoginRegistration");
            }
            return View();
        }

        //adding department
        [HttpPost]
        public async Task<IActionResult> Add(DepartmentViewModel dept)
        {
            if (!ModelState.IsValid)
            {
                return View(dept);
            }
            bool departmentExits = await dbContext.Departments.AnyAsync(d => d.Name == dept.Name);
            if (departmentExits)
            {
                TempData["insertSuccess"] = "Department already exits";
                return View(dept);
            }
            var newDepartment = new Department()
            {
                Name = dept.Name
            };
            await dbContext.Departments.AddAsync(newDepartment);
            await dbContext.SaveChangesAsync();
            TempData["insertSuccess"] = "Department added successfully";
            return RedirectToAction("Add", "Department");
            //return Redirect("https://www.google.com/");

        }
        //employees who have same departament
        public async Task<IActionResult> DeptDetail(int deptId)
        {
            // No need for a null check on dbContext.Departments, EF Core will handle this internally
            //if(dbContext.Departments == null)
            //{
            //    return NotFound();
            //}
            var result = await dbContext.Departments
                         .Include(d => d.Employees)//include related employees
                         .Where(d => d.DepartmentId == deptId)// Filters by DepartmentId
                         .Select(d => new DepartementEmpViewModel
                         {
                             DepartmentId = d.DepartmentId,
                             DepartmentName = d.Name,
                             Employees = d.Employees.ToList()
                         })
                         .FirstOrDefaultAsync();
            if (result == null)
            {
                return NotFound();
            }
            //return Json(result);
            return View(result);
        }
        //for printing departmentList
        public IActionResult PopupContent()
        {
            return View();
        }

        //update department UI
        public async Task<IActionResult> Update(int deptId)
        {
            if (dbContext.Departments == null)
            {
                return NotFound("Record not found");
            }

            //var department = await dbContext.Departments.FindAsync(deptId);
            //var department = await dbContext.Departments.FirstOrDefaultAsync(d=>d.DepartmentId==deptId);
            var department = await dbContext.Departments
                             .Where(d => d.DepartmentId == deptId)
                             .Select(d => new DepartmentViewModel
                             {
                                 DepartmentId = d.DepartmentId,
                                 Name = d.Name
                             })
                             .FirstOrDefaultAsync();
            if (department == null)
            {

                return NotFound("No Record Found");
            }
            //no need with select query but required with Find
            //DepartmentViewModel newDepartment = new DepartmentViewModel
            //{
            //    DepartmentId =  department.DepartmentId,
            //    Name = department.Name
            //};
            return View(department);
        }

        //update Department
        [HttpPost]
        public async Task<IActionResult> Update(DepartmentViewModel dept)
        {
            if (!ModelState.IsValid)
            {
                return Json(dept);
            }
            bool departmentExists = await dbContext.Departments.AnyAsync(d => d.Name == dept.Name && d.DepartmentId != dept.DepartmentId);
            if (departmentExists)
            {
                TempData["duplicateDepartment"] = "This department already exits";
                return View(dept);
            }
            var department = await dbContext.Departments.FindAsync(dept.DepartmentId);
            if (department == null)
            {
                return NotFound();
            }
            //to check for changes
            bool hasChanges = false;
            if (department.Name != dept.Name)
            {
                department.Name = dept.Name;
                hasChanges = true;
            }
            if (hasChanges)
            {
                await dbContext.SaveChangesAsync();
                TempData["editSuccess"] = "Department updated successfully";
            }
            else
            {
                TempData["editSuccess"] = "No changes detected";
            }
            return RedirectToAction("Index", "Department");
        }
        //Delete department
        public async Task<IActionResult> Delete(int deptId)
        {
            //var department = await dbContext.Departments.FindAsync(deptId);
            var department = await dbContext.Departments
                                   .Include(d => d.Employees)// Include related employees
                                   .FirstOrDefaultAsync(d => d.DepartmentId == deptId);
            if (department == null)
            {
                return NotFound();
            }
            //check if the department has any associated employees
            if (department.Employees != null && department.Employees.Any())
            {
                TempData["DeleteError"] = "Department can not be deleted as it has associated employees";
                return RedirectToAction("Index", "Department");
            }

            // Proceed with deletion if no employees are associated
            dbContext.Departments.Remove(department);
            await dbContext.SaveChangesAsync();
            TempData["DeleteSucces"] = "Department deleted successfully";
            //return Json(deptId);
            return RedirectToAction("Index", "Department");
         }
        //Department consolidated Report
        public async Task<IActionResult> DepartmentConsolidateReport()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserSession")))
            {
                return RedirectToAction("Login", "UserLoginRegistration");
            }
            //[JsonIgnore] Prevents circular reference during serialization
            var departments = await dbContext.Departments
                          .Include(d=> d.Employees)
                         .ToListAsync();

            return View(departments); 
        }
        //department list export excel 
        public async Task<IActionResult> ExportDeptExcel()
        {
            var result = await dbContext.Departments.ToListAsync();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Departments");
                var currentRow = 1;
                // add header
                worksheet.Cell(currentRow, 1).Value = "#";
                worksheet.Cell(currentRow, 2).Value = "Dept ID";
                worksheet.Cell(currentRow, 3).Value = "Department Name";

                // Apply bold styling to the header
                for (int col = 1; col <= 3; col++)
                {
                    worksheet.Cell(currentRow, col).Style.Font.Bold = true;
                }

                //add rows
                int srNo = 1;
                foreach(var department in result)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = srNo;
                    worksheet.Cell(currentRow, 2).Value = department.DepartmentId;
                    worksheet.Cell(currentRow, 3).Value = department.Name;
                    srNo++;
                }

                //Save to a memory stream
                using (var stream =  new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Departments.xlsx");
                }
            }
            //return Json(result);
        }
        //print departments
        public async Task<IActionResult> PrintDepartment()
        {
            var departments = await dbContext.Departments
                .Include(d=>d.Employees)
                             .Select(d=>new DepartmentViewModel
                             {
                                DepartmentId= d.DepartmentId,
                                Name= d.Name,                                
                             }).ToListAsync();
            return View(departments);
         }
    }
}

