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

            using (var stream = new MemoryStream())//1.	Creates a temporary "file" in-memory or in-memoryfile.
            {
                DeptExlFile.CopyTo(stream);// copy uploaded file data into temporary in-memory file

                using (var package = new ExcelPackage(stream))// Load Excel data into package
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];//creating a variable worksheet that refers to the first worksheet in the Excel file, to read or manipulate data in that sheet.
                    int rowCount = worksheet.Dimension.Rows;//how many rows contain data, including any headers or actual values
                    for (int row = 2; row <= rowCount; row++)
                    {
                        departments.Add(new Department
                        {
                            Name = worksheet.Cells[row, 1].Text.Trim()//get data from Cell which has row2 and col1
                        });
                    }
                }

            }

            //return Json(new {Name = DeptExlFile.FileName, Size = DeptExlFile.Length, ext=extension});
           return Json(departments);
        }
    }
}


