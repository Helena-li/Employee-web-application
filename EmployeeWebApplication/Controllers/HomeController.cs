using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EmployeeWebApplication.Models;
using EmployeeWebApplication.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EmployeeWebApplication.Controllers
{
    public class HomeController : Controller
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IHostingEnvironment hostingEnvironment;

        public HomeController(IEmployeeRepository employeeRepository,
            SignInManager<ApplicationUser> signInManager
            , IHostingEnvironment hostingEnvironment)
        {
            this._employeeRepository = employeeRepository;
            this.signInManager = signInManager;
            this.hostingEnvironment = hostingEnvironment;
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

        [HttpGet]
        public ViewResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(EmployeeCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                string uniqueName = ProcessUploadedFiles(model);
                Employee emp = new Employee
                {
                    Name = model.Name,
                    Email = model.Email,
                    Department = model.Department,
                    PhotoPath = uniqueName
                };
                _employeeRepository.Add(emp);
                return RedirectToAction("details", new { id = emp.Id });
            }
            return View();
        }

        private string ProcessUploadedFiles(EmployeeCreateViewModel model)
        {
            string uniqueName = null;
            if (model.Photo != null)
            {
                //WebRootPath get wwwroot folder, reture the image path as a string
                string uploadFolder = Path.Combine(hostingEnvironment.WebRootPath, "images");
                uniqueName = Guid.NewGuid().ToString() + "_" + model.Photo.FileName;
                string filePath = Path.Combine(uploadFolder, uniqueName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.Photo.CopyTo(fileStream);
                }
            }

            return uniqueName;
        }

        [HttpGet]
        public ViewResult Edit(int id)
        {
            var emp = _employeeRepository.GetEmployee(id);
            EmployeeEditViewModel employee = new EmployeeEditViewModel
            {
                Id = emp.Id,
                Name = emp.Name,
                Email = emp.Email,
                Department = emp.Department,
                ExistingPhotoPath = emp.PhotoPath
            };
            return View(employee);
        }

        [HttpPost]
        public IActionResult Edit(EmployeeEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var emp = _employeeRepository.GetEmployee(model.Id);
                emp.Name = model.Name;
                emp.Email = model.Email;
                emp.Department = model.Department;

                string uniqueName = ProcessUploadedFiles(model);
                if (model.ExistingPhotoPath != null)
                {
                    string existingPath = Path.Combine(hostingEnvironment.WebRootPath, "images", model.ExistingPhotoPath);
                    System.IO.File.Delete(existingPath);
                }

                emp.PhotoPath = uniqueName;
                _employeeRepository.Update(emp);
                return RedirectToAction("index");
            }
            return View();
        }

        
        public IActionResult Delete(int id)
        {
            var emp = _employeeRepository.GetEmployee(id);
            if (emp == null)
            {
                ViewBag.ErrorTitle = "Error";
                ViewBag.ErrorMessage = "Cannot find the employee";
                return View("Error");
            }
            var result = _employeeRepository.Delete(id);
            return RedirectToAction("index");
        }

    }
}
