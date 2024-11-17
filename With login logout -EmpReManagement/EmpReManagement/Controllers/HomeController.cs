
using EmpReManagement.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;

namespace EmpReManagement.Controllers
{
    public class HomeController : Controller
    {
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public IActionResult Index()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserSession")))
            {
                return RedirectToAction("Login", "UserLoginRegistration");
            }
            return View();
        }             
       
    }
}
