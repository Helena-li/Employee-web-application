using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmployeeWebApplication.Models;
using EmployeeWebApplication.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EmployeeWebApplication.Controllers
{
    public class HomeController : Controller
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly SignInManager<ApplicationUser> signInManager;

        public HomeController(IEmployeeRepository employeeRepository,
            SignInManager<ApplicationUser> signInManager)
        {
            this._employeeRepository = employeeRepository;
            this.signInManager = signInManager;
        }
        
        [AllowAnonymous]
        public IActionResult Index()
        {
            var model = _employeeRepository.GetEmployeeList();
            return View(model);
        }

        public ViewResult Details(int id)
        {
            var employee = _employeeRepository.GetEmployee(id);
            if (employee == null)
            {
                Response.StatusCode = 404;
                return View("EmployeeNotFound", id);
            }
            HomeDetailViewModels homeDetailViewModels = new HomeDetailViewModels()
            {
                Employee = employee,
                Title = "Emplyee Details"
            };

            return View(homeDetailViewModels);
        }
    }
}
