
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using EmpReManagement.Data;
using EmpReManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;


namespace EmpReManagement.Controllers
{
    public class ImportEmployeeController : Controller
    {
        private readonly AppDbContext dbContext;

        public ImportEmployeeController(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        [HttpGet]
        public async Task<IActionResult> ImportEmployee()
        {
            if(string.IsNullOrEmpty(HttpContext.Session.GetString("UserSession")))
            {
                return RedirectToAction("Login", "UserLoginRegistration");
            }
            return View();
            //return Json(await dbContext.Employees.Include(e=>e.Departments).ToListAsync());
        }


        //for importing employee for bulk entry
        [HttpPost]
        public async Task<IActionResult> ImportEmployee(IFormFile EmpExclFile)
        {
            if (EmpExclFile == null || EmpExclFile.Length == 0)
            {
                TempData["EmpImportError"] = "File could not get uploaded";
                return View();
            }
            var extension =  Path.GetExtension(EmpExclFile.FileName).ToLowerInvariant();
            if (extension != ".xls" && extension != ".xlsx")
            {
                TempData["EmpImportError"] = "Invalid file format. Please select an Excel file.";
                return View();
            }
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // Set the license context that you’re using EPPlus under a non-commercial license context.
           
            var employees = new List<Employee>();
           
            using (var stream = new MemoryStream())
            {
                EmpExclFile.CopyTo(stream);
                using(var package = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                    int rowCount = worksheet.Dimension.Rows;
                    for(int row =2; row<=rowCount; row++)
                    {
                        employees.Add(new Employee
                        {
                            FirstName = worksheet.Cells[row, 1].Text.Trim(),// Get the cell value as a string
                            LastName = worksheet.Cells[row, 2].Text.Trim(),
                            DateOfBirth = DateOnly.Parse(worksheet.Cells[row, 3].Text.Trim()),
                            Gender = worksheet.Cells[row, 4].Text.Trim(),
                            Email = worksheet.Cells[row, 5].Text.Trim(),
                            PhoneNumber = worksheet.Cells[row, 6].Text.Trim(),
                            Address = worksheet.Cells[row, 7].Text.Trim(),
                            IsActive = bool.Parse(worksheet.Cells[row,8].Text.Trim()),
                            DepartmentId = int.Parse(worksheet.Cells[row,9].Text.Trim())
                        });
                    }
                }
            }
           //await dbContext.Employees.AddRangeAsync(employees);
           //await dbContext.SaveChangesAsync();

          return Json(employees);
        }
    }
}

