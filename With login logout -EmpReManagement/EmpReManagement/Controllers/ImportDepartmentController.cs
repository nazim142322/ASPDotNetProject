using EmpReManagement.Data;
using EmpReManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.IO.Packaging;

namespace EmpReManagement.Controllers
{
    public class ImportDepartmentController : Controller
    {
        private readonly AppDbContext dbContext;

        public ImportDepartmentController(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }     

        public async Task<IActionResult> ImportDepartment()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserSession")))
            {
                return RedirectToAction("Login", "UserLoginRegistration");
            }
            //var result = await dbContext.Departments.Include(d => d.Employees).ToListAsync();
            return View();
        }

        [HttpPost]
        public async Task <IActionResult> ImportDepartment(IFormFile DeptExlFile)
        {
            if(DeptExlFile==null && DeptExlFile.Length==0)
            {
                TempData["DeptImportError"] = "File could not get uploaded";
                return View();
            }
            var extension = Path.GetExtension(DeptExlFile.FileName).ToLowerInvariant();
          
            if(extension != ".xls" && extension != ".xlsx")
            {
               TempData["DeptImportError"] = "Please select excel file such as '.xls' or '.xlsx'";
                return View();
            }
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            // Set the license context that you’re using EPPlus under a non-commercial license context.

            var departments = new List<Department>();
            var duplicateDepartments = new List<String>();

            using (var stream = new MemoryStream())//1.	Creates a temporary "file" in-memory or in-memoryfile.
            {
                DeptExlFile.CopyTo(stream);// copy uploaded file data into temporary in-memory file

                using (var package = new ExcelPackage(stream))//ExcelPackage reads stream and Load Excel data into package
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];//creating a variable worksheet that refers to the first worksheet in the Excel file, to read or manipulate data in that sheet.
                    int rowCount = worksheet.Dimension.Rows;//how many rows contain data, including any headers or actual values
                    for (int row = 2; row <= rowCount; row++)
                    {
                        string deptName = worksheet.Cells[row, 1].Text.Trim();
                        // Check if department already exists in the database
                        bool deptExits = await dbContext.Departments.AnyAsync(d => d.Name == deptName);
                        if(!deptExits)
                        {
                            departments.Add(new Department{Name = deptName});
                        }
                        else
                        {
                            duplicateDepartments.Add(deptName);
                        }                        
                    }
                }
            }  
            if(departments.Any())
            {
                await dbContext.Departments.AddRangeAsync(departments);
                await dbContext.SaveChangesAsync();
                TempData["DepartmentsAdded"] = "Departments added successfully";
                ViewBag.Departments = departments;
            }
            if(duplicateDepartments.Any())
            {
                TempData["DeptImportError"] = $"Some departments were not added because they already exist: {string.Join(", ", duplicateDepartments)}";
            }                    

            //return Json(new {Name = DeptExlFile.FileName, Size = DeptExlFile.Length, ext=extension});
            return View();
        }
    }
}


