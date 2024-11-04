using EmpReManagement.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace EmpReManagement.Controllers
{
    public class ImportDepartmentController : Controller
    {
        private readonly AppDbContext dbContext;

        public ImportDepartmentController(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public IActionResult Index()
        {
            return View();
        }


        public async Task<IActionResult> ImportDepartment()
        {
            //var result = await dbContext.Departments.Include(d => d.Employees).ToListAsync();
            return View();
        }
        [HttpPost]
        public async Task <IActionResult> ImportDepartment(IFormFile DeptExlFile)
        {
            if (DeptExlFile == null || DeptExlFile.Length == 0)
                return BadRequest("No file uploaded");

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // Set the license context that you’re using EPPlus under a non-commercial license 
            var departments = new List<Dictionary<string, string>>();

            using (var stream = new MemoryStream())
            {
                await DeptExlFile.CopyToAsync(stream);
                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets[0]; // Access the first worksheet
                    var rowCount = worksheet.Dimension.Rows;
                    var colCount = worksheet.Dimension.Columns;

                    // Assuming first row contains headers
                    var headers = new List<string>();
                    for (int col = 1; col <= colCount; col++)
                    {
                        headers.Add(worksheet.Cells[1, col].Text);
                    }

                    // Read rows and convert to dictionary
                    for (int row = 2; row <= rowCount; row++)
                    {
                        var department = new Dictionary<string, string>();
                        for (int col = 1; col <= colCount; col++)
                        {
                            department[headers[col - 1]] = worksheet.Cells[row, col].Text;
                        }
                        departments.Add(department);
                    }
                }
            }
            return Json(departments);
        }
    }
}
