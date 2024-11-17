using EmpReManagement.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using EmpReManagement.ViewModel;
using EmpReManagement.Models;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace EmpReManagement.Controllers
{
    public class UserLoginRegistrationController : Controller
    {
        private readonly AppDbContext dbcontext;

        public UserLoginRegistrationController(AppDbContext dbcontext)
        {
            this.dbcontext = dbcontext;
        }
        public async Task <IActionResult> Login()
        {
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UserSession")))
            {
                return RedirectToAction("Index", "Home");
            }            
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string Email, string Password)
        {
            var myUser = await dbcontext.Users.Where(x=>x.Email==Email && x.Password==Password).FirstOrDefaultAsync();
            if(myUser!=null)
            {
                HttpContext.Session.SetString("UserSession", myUser.Name);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["LoginMessage"] = "Email address or password is incorrect !";
            }
            return View("Login");
        }

        [HttpGet]
        public async Task <IActionResult> Registration()
        {
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UserSession")))
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registration(UserViewModel model)
        {            
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = new User()
            {
                Name =  model.Name.Trim(),
                PhoneNo =  model.PhoneNo.Trim(),
                Email = model.Email.Trim(),
                Password = model.Password,
            };
            await dbcontext.Users.AddAsync(user);
            await dbcontext.SaveChangesAsync();
            TempData["RegistrationMessage"] = "User added successfully";
            return RedirectToAction("Login");           
        }
        public IActionResult LogOut()
        {
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UserSession")))
            {
                HttpContext.Session.Clear(); // Removes all keys in the session               
            }
            return RedirectToAction("Login", "UserLoginRegistration");
        }
    }
}
