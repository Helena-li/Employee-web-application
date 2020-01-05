using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmployeeWebApplication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EmployeeWebApplication.Controllers
{
    public class HomeController : Controller
    {
        private readonly IEmployeeRepository employeeRepository;
        private readonly SignInManager<ApplicationUser> signInManager;

        public HomeController(IEmployeeRepository employeeRepository,
            SignInManager<ApplicationUser> signInManager)
        {
            this.employeeRepository = employeeRepository;
            this.signInManager = signInManager;
        }
        
        [AllowAnonymous]
        public IActionResult Index()
        {
            var model = employeeRepository.GetEmployeeList();
            return View(model);
        }
    }
}
