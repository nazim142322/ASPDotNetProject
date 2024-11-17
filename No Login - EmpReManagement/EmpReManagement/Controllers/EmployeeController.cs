using AutoMapper;
using ClosedXML.Excel;
using EmpReManagement.Data;
using EmpReManagement.Models;
using EmpReManagement.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.IO;
using static System.Net.Mime.MediaTypeNames;
namespace EmpReManagement.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly AppDbContext dbContext;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment env;

        public EmployeeController(AppDbContext dbContext, IMapper mapper, IWebHostEnvironment env)
        {
            this.dbContext = dbContext;
            _mapper = mapper;
            this.env = env;
        }

        //Getting records
        public async Task<IActionResult> Index(string searchString, string sortOrder)
        {
            var employees = await dbContext.Employees.ToListAsync();
            // If no employees were found, return NotFound
            //if (employees == null || employees.Count == 0)
            //{
            //    return NotFound("No record found");
            //}
            //Perform case-insensitive search
            if (!string.IsNullOrEmpty(searchString))
            {
                string lowerCaseSearchString = searchString.ToLower();
                employees = employees.Where(n => n.FirstName.ToLower().Contains(lowerCaseSearchString) || n.LastName.ToLower().Contains(lowerCaseSearchString)).ToList();
            }
            //ViewData["SortOrder"] = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["SortOrder"] = sortOrder == "name_desc" ? "name_asc" : "name_desc";
            ViewData["IdSortOrder"] = sortOrder == "id_desc" ? "id_asc" : "id_desc";
            ViewData["GenSortOrder"] = sortOrder == "gen_desc" ? "gen_asc" : "gen_desc";
            ViewData["ActSortOrder"] = sortOrder == "act_desc" ? "act_asc" : "act_desc";
            ViewData["DeptSortOrder"] = sortOrder == "dept_desc" ? "dept_asc" : "dept_desc";
            ViewData["dobSortOrder"] = sortOrder == "dob_desc" ? "dob_asc" : "dob_desc";

            //Apply sorting
            switch (sortOrder)
            {
                case "name_desc":
                    employees = employees.OrderByDescending(e => e.FirstName).ToList();
                    break;
                case "name_asc":
                    employees = employees.OrderBy(e => e.FirstName).ToList();
                    break;

                case "id_desc":
                    employees = employees.OrderByDescending(e => e.EmployeeId).ToList();
                    break;
                case "id_asc":
                    employees = employees.OrderBy(e => e.EmployeeId).ToList();
                    break;
                case "gen_desc":
                    employees = employees.OrderByDescending(e => e.Gender).ToList();
                    break;
                case "gen_asc":
                    employees = employees.OrderBy(e => e.Gender).ToList();
                    break;

                case "act_desc":
                    employees = employees.OrderByDescending(e => e.IsActive).ToList();
                    break;
                case "act_asc":
                    employees = employees.OrderBy(e => e.IsActive).ToList();
                    break;

                case "dept_desc":
                    employees = employees.OrderByDescending(e => e.DepartmentId).ToList();
                    break;
                case "dept_asc":
                    employees = employees.OrderBy(e => e.DepartmentId).ToList();
                    break;

                case "dob_desc":
                    employees = employees.OrderByDescending(e => e.DateOfBirth).ToList();
                    break;
                case "dob_asc":
                    employees = employees.OrderBy(e => e.DateOfBirth).ToList();
                    break;

                default://default sorting
                    employees = employees.OrderBy(e => e.EmployeeId).ToList();
                    break;
            }

            // Map the list of Employee entities to a list of EmployeeViewModel
            var newData = _mapper.Map<List<EmployeeViewModel>>(employees);
            return View(newData);
        }


        //GET : Employee/Add
        public IActionResult Add()
        {
            //List<SelectListItem> departments = new List<SelectListItem>
            //{
            //    new SelectListItem{Value="1", Text="IT"},
            //    new SelectListItem{Value="2", Text="Sales"},
            //    new SelectListItem{Value="3", Text="Accounts"},
            //};
            //ViewBag.departments = departments;
            EmployeeViewModel empModel = new EmployeeViewModel
            {
                Departments = new List<SelectListItem>()
            };
            var data = dbContext.Departments.OrderBy(d=>d.Name).ToList();
            if(data!=null)
            {
                int count = 1;
                foreach (var item in data)
                {
                   empModel.Departments.Add(new SelectListItem { Value = item.DepartmentId.ToString(), Text = $"{count++}. {item.Name}" });
                }
                ViewBag.departments = empModel.Departments;
            }            
            return View();
        }

        //POST : Employee/Add
        [HttpPost]
        public async Task<IActionResult> Add(EmployeeViewModel emp)
        {
            if (ModelState.IsValid == false)
            {
                // Re-populate the ViewBag to ensure dropdown options are still available
                EmployeeViewModel empModel = new EmployeeViewModel
                {
                    Departments = new List<SelectListItem>()
                };
                var data = dbContext.Departments.ToList();
                foreach (var item in data)
                {
                    empModel.Departments.Add(new SelectListItem { Value = item.DepartmentId.ToString(), Text = item.Name });
                }

                ViewBag.departments = empModel.Departments;
                return View(emp);
            }

            //Handle file upload
            string uniqueFileName = null;

            if (emp.Photo != null && emp.Photo.Length > 0)
            {
                var extension = Path.GetExtension(emp.Photo.FileName).ToLowerInvariant();
                var size = emp.Photo.Length;//length in bytes                 
                // Only allow jpg, png, jpeg extensions
                if (extension == ".png" || extension == ".jpg" || extension == ".jpeg")
                {    
                    if(size<=2000000)
                    {
                        string uploadsFolder = Path.Combine(env.WebRootPath, "EmpImages");
                        uniqueFileName = Guid.NewGuid().ToString() + "_" + emp.Photo.FileName;
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);                       
                        emp.Photo.CopyTo(new FileStream(filePath, FileMode.Create));
                    }
                    else
                    {
                        TempData["UploadFileSize_Eroor"] = "Photo Size Should less than 2MB";
                        return View(emp);
                    }                            
                }
                else
                {
                    TempData["extension_error"] = "Only jpg, png and jpeg images are allowed";
                    return View(emp);
                }

            }
            else
            {
                TempData["PhotoUploadError"] = "Photo could not get uploaded";
            }
                // Map the EmployeeViewModel to Employee entity
                var newEmp = _mapper.Map<Employee>(emp);

                if (uniqueFileName != null)
                {
                    newEmp.ImagePath = uniqueFileName;
                }
                await dbContext.Employees.AddAsync(newEmp);
                await dbContext.SaveChangesAsync();

                TempData["insertSuccess"] = "Employee added successfully";
                return RedirectToAction("Add", "Employee");
                // return Json(newEmp);            
        }

        //Employee details
        public async Task<IActionResult> EmpDetails(int empId)
        {
            if(empId==null || dbContext.Employees==null)
            {
                return NotFound();
            }
            //var empData = await dbContext.Employees.FindAsync(empId);
            // LINQ query for INNER JOIN between Employees and Departments(LINQ Equivalent of SQL Query)       
           //LINQ with Navigation Properties(Simpler Approach):
            var empData = await dbContext.Employees
                                 .Include(e => e.Departments)
                                 .Where(e => e.EmployeeId == empId)
                                 .Select(e => new EmployeeDetailsVeiwModel
                                 {
                                     EmployeeId = e.EmployeeId,
                                     FirstName = e.FirstName,
                                     LastName = e.LastName,
                                     DateOfBirth = e.DateOfBirth,
                                     Gender = e.Gender,
                                     Email = e.Email,
                                     PhoneNumber = e.PhoneNumber,
                                     Address = e.Address,
                                     IsActive = e.IsActive,
                                     ImagePath = e.ImagePath,
                                     DepartmentName = e.Departments.Name  // Map the Department Name
                                 })
                                 .FirstOrDefaultAsync();

            if (empData==null)
            {
                return NotFound();
            }      
           //Return the data as JSON
            return View(empData);
        }

        //update record UI
        public async Task<IActionResult> Update(int empId)
        {
            EmployeeViewModel depts = new EmployeeViewModel
            {
                Departments = new List<SelectListItem>()
            };
            var ddldata = await dbContext.Departments.OrderBy(d=>d.Name).ToListAsync();
            if(ddldata!=null)
            {
                foreach (var item in ddldata)
                {
                    depts.Departments.Add(new SelectListItem { Value = item.DepartmentId.ToString(), Text = item.Name });
                }
                ViewBag.departments = depts.Departments;
            }

            // No need to check empId == null, since it's an int (value type)
            if (empId==null || dbContext.Employees == null)
            {
                return NotFound();
            }
            var empData = await dbContext.Employees.FindAsync(empId);

            if(empData==null)
            {
                return NotFound();
            }
            var newEmpData = new EmployeeViewModel
            {
                EmployeeId = empData.EmployeeId,
                FirstName= empData.FirstName,
                LastName =  empData.LastName,
                DateOfBirth = empData.DateOfBirth,
                Gender = empData.Gender,
                Email= empData.Email,
                PhoneNumber = empData.PhoneNumber,
                Address =empData.Address,
                IsActive = empData.IsActive,
                ImagePath = empData.ImagePath,
                DepartmentId = empData.DepartmentId
            };
            //var newEmpData = _mapper.Map<EmployeeViewModel>(empData);
            return View(newEmpData);
        }

        //update record post method
        [HttpPost]
        public async Task<IActionResult> Update(EmployeeViewModel emp)
        {

            if (!ModelState.IsValid)
            {
                EmployeeViewModel depts = new EmployeeViewModel
                {
                    Departments = new List<SelectListItem>()
                };
                var ddldata = await dbContext.Departments.OrderBy(d => d.Name).ToListAsync();
                if (ddldata != null)
                {
                    foreach (var item in ddldata)
                    {
                        depts.Departments.Add(new SelectListItem { Value = item.DepartmentId.ToString(), Text = item.Name });
                    }
                    ViewBag.departments = depts.Departments;
                }
                return Json(emp);  //Validation failed, return the view with current model
            }
            var empData =  await dbContext.Employees.FindAsync(emp.EmployeeId);
            if (empData == null)
            {
                return NotFound("Records Not Found");
            }
            // Map the updated view model to the existing employee entity
            empData.FirstName = emp.FirstName;
            empData.LastName = emp.LastName;
            empData.DateOfBirth = emp.DateOfBirth;
            empData.Gender = emp.Gender;
            empData.Email = emp.Email;
            empData.PhoneNumber = emp.PhoneNumber;
            empData.Address = emp.Address;
            empData.DepartmentId = emp.DepartmentId;
            empData.IsActive = emp.IsActive;

            // Handle the photo upload if a new file is uploaded
            string newImageFileName = null;

            if (emp.Photo != null && emp.Photo.Length > 0)
            {
                var extension = Path.GetExtension(emp.Photo.FileName).ToLowerInvariant();
                var size = emp.Photo.Length;//length in bytes
                                            
                // Only allow jpg, png, jpeg extensions
                if (extension == ".png" || extension == ".jpg" || extension == ".jpeg")
                {
                    if (size <= 2000000)
                    {
                        // Path to the folder where images are stored
                        string uploadsFolder = Path.Combine(env.WebRootPath, "EmpImages");
                        
                        // Delete the old image if it exists
                        if (!string.IsNullOrEmpty(empData.ImagePath))
                        {
                            string oldImagePath = Path.Combine(uploadsFolder, empData.ImagePath);
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath); // Delete old image
                            }
                        }

                        // Save the new image
                        newImageFileName = Guid.NewGuid().ToString() + "_" + emp.Photo.FileName;
                        string newfilePath = Path.Combine(uploadsFolder, newImageFileName);                       
                        using (var stream = new FileStream(newfilePath, FileMode.Create))
                        {
                            await emp.Photo.CopyToAsync(stream);
                        }
                    }
                    else
                    {
                        TempData["UploadFileSize_Eroor"] = "Photo Size Should less than 2MB";
                        return View(emp);
                    }
                }
                else
                {
                    TempData["extension_error"] = "Only jpg, png and jpeg images are allowed";
                    return View(emp);
                }
            }
            //else
            //{
            //    TempData["PhotoUploadError"] = "Photo could not get uploaded";
            //}
            // Update the employee image path in the database
            if (newImageFileName != null)
            {
                empData.ImagePath = newImageFileName;
            };       

            dbContext.Employees.Update(empData);
            await dbContext.SaveChangesAsync();
            TempData["UpdateSuccess"] = "Record Updated Successfully";
            return RedirectToAction("EmpDetails", "Employee", new {empId = emp.EmployeeId });
            //https://localhost:7034/Employee/EmpDetails?empId=1

        }
        //delete employee record
        public async Task<IActionResult> DeleteRecord(int empId)
        {
            var result = await dbContext.Employees.FindAsync(empId);
            if(result!=null)
            {
                dbContext.Employees.Remove(result);
               await  dbContext.SaveChangesAsync();
                TempData["DeleteEmpSuccess"] = "Employee Record Deleted!";
                return RedirectToAction("Index", "Employee");
            }
            return Json($"hello world {empId}");
        }
        //print employee with details - consolidated sheet
        public async Task<IActionResult> PrintEmpDetail()
        {
            var employees = await dbContext.Employees
                             .Include(e => e.Departments)
                              .Select(e => new EmployeeDetailsVeiwModel
                              {
                                  EmployeeId = e.EmployeeId,
                                  FirstName = e.FirstName,
                                  LastName = e.LastName,
                                  DateOfBirth = e.DateOfBirth,
                                  Gender = e.Gender,
                                  Email = e.Email,
                                  PhoneNumber = e.PhoneNumber,
                                  Address = e.Address,
                                  IsActive=e.IsActive,
                                  DepartmentName=e.Departments.Name

                              }).ToListAsync();
            return View(employees);
        }
        //export Excel
        public async Task<IActionResult> ExcelExport()
        {
            var employees = await dbContext.Employees
                   .Include(e => e.Departments)
                   .ToListAsync(); // Retrieve employees from dbContext

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Employees");

                var currentRow = 1;

                // Add header
                worksheet.Cell(currentRow, 1).Value = "#";
                worksheet.Cell(currentRow, 2).Value = "EmployeeId";
                worksheet.Cell(currentRow, 3).Value = "FirstName";
                worksheet.Cell(currentRow, 4).Value = "LastName";
                worksheet.Cell(currentRow, 5).Value = "DateOfBirth (mm-dd-yy)";
                worksheet.Cell(currentRow, 6).Value = "Gender";
                worksheet.Cell(currentRow, 7).Value = "Email";
                worksheet.Cell(currentRow, 8).Value = "PhoneNumber";
                worksheet.Cell(currentRow, 9).Value = "Address";
                worksheet.Cell(currentRow, 10).Value = "IsActive";
                worksheet.Cell(currentRow, 11).Value = "ImagePath";
                worksheet.Cell(currentRow, 12).Value = "DepartmentId";
                worksheet.Cell(currentRow, 13).Value = "DepartmentName";

                // Apply bold styling to the header
                for (int col = 1; col <= 13; col++)
                {
                    worksheet.Cell(currentRow, col).Style.Font.Bold = true;
                }
                // Add rows
                int srNo = 1;
                foreach (var employee in employees)
                {
                    currentRow++;//currentRow is incremented, creating a new row for each record.
                    worksheet.Cell(currentRow, 1).Value = srNo;
                    worksheet.Cell(currentRow, 2).Value = employee.EmployeeId;
                    worksheet.Cell(currentRow, 3).Value = employee.FirstName;
                    worksheet.Cell(currentRow, 4).Value = employee.LastName;
                    worksheet.Cell(currentRow, 5).Value = employee.DateOfBirth.ToString();
                    worksheet.Cell(currentRow, 6).Value = employee.Gender;
                    worksheet.Cell(currentRow, 7).Value = employee.Email;
                    worksheet.Cell(currentRow, 8).Value = employee.PhoneNumber;
                    worksheet.Cell(currentRow, 9).Value = employee.Address;
                    worksheet.Cell(currentRow, 10).Value = (employee.IsActive?"Active":"Not Active");
                    worksheet.Cell(currentRow, 11).Value = employee.ImagePath;
                    worksheet.Cell(currentRow, 12).Value = employee.DepartmentId;
                    worksheet.Cell(currentRow, 13).Value = employee.Departments.Name;
                    srNo++;
                }

                // Save to a memory stream
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();

                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Employees.xlsx");
                }
            }
        }
    }
}
