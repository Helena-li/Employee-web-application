using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmployeeWebApplication.Models;
using EmployeeWebApplication.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EmployeeWebApplication.Controllers
{
    public class AdminController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<ApplicationUser> userManager;

        public AdminController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
        }

        [HttpGet]
        public IActionResult ListRole()
        {
            var result = roleManager.Roles;
            return View(result);
        }

        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityRole identity = new IdentityRole
                {
                    Name = model.RoleName
                };
                var result = await roleManager.CreateAsync(identity);
                if (result.Succeeded)
                {
                    return RedirectToAction("listrole", "admin");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditRole(string id)
        {
            var result = await roleManager.FindByIdAsync(id);
            if (result == null)
            {
                ViewBag.ErrorMessage = $"role with id = {id} is not found";
                return RedirectToAction("NotFound");
            }
            else
            {
                EditRoleViewModel editRoleViewModel = new EditRoleViewModel
                {
                    Id = id,
                    RoleName = result.Name
                };
                foreach (var user in userManager.Users)
                {
                    if (await userManager.IsInRoleAsync(user, result.Name))
                    {
                        editRoleViewModel.Users.Add(user.UserName);
                    }
                }
                return View(editRoleViewModel);
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditRole(EditRoleViewModel model)
        {
            var role = await roleManager.FindByIdAsync(model.Id);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"role with id = {model.Id} is not found";
                return RedirectToAction("NotFound");
            }
            else
            {
                role.Name = model.RoleName;
                var result = await roleManager.UpdateAsync(role);
                if (result.Succeeded)
                {
                    return RedirectToAction("listrole", "admin");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View(model);
                }
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditUsersInRole(string roleId)
        {
            ViewBag.RoleId = roleId;
            var role = await roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"role with id = {roleId} is not found";
                return RedirectToAction("NotFound");
            }
            else
            {
                ViewBag.RoleName = role.Name;
                var model = new List<UserRoleViewModel>();
                foreach (var user in userManager.Users)
                {
                    UserRoleViewModel userRoleViewModel = new UserRoleViewModel
                    {
                        UserId = user.Id,
                        UserName = user.UserName
                    };
                    if (await userManager.IsInRoleAsync(user, role.Name))
                    {
                        userRoleViewModel.IsSelected = true;
                    }
                    else
                    {
                        userRoleViewModel.IsSelected = false;
                    }
                    model.Add(userRoleViewModel);
                }
                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditUsersInRole(List<UserRoleViewModel> model, string roleId)
        {
            var role = await roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"role with id = {roleId} is not found";
                return RedirectToAction("NotFound", "home");
            }
            else
            {
                for (int i = 0; i < model.Count; i++)
                {
                    var user = await userManager.FindByIdAsync(model[i].UserId);
                    IdentityResult result = null;
                    if (model[i].IsSelected && !(await userManager.IsInRoleAsync(user, role.Name)))
                    {
                        result = await userManager.AddToRoleAsync(user, role.Name);
                    }
                    else if (!model[i].IsSelected && (await userManager.IsInRoleAsync(user, role.Name)))
                    {
                        result = await userManager.RemoveFromRoleAsync(user, role.Name);
                    }
                    else
                    {
                        continue;
                    }
                    if (result.Succeeded)
                    {
                        if (i < (model.Count - 1))
                        {
                            continue;
                        }
                        else
                        {
                            return RedirectToAction("EditRole", new { Id = roleId });
                        }
                    }
                }
                return RedirectToAction("EditRole", new { Id = roleId });
            }
        }


        [HttpGet]
        public IActionResult ListUsers()
        {
            var result = userManager.Users;
            return View(result);
        }

    }
}
