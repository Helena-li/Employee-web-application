using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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

        #region role
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

        [HttpPost]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = await roleManager.FindByIdAsync(id);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"role with id = {id} is not found";
                return RedirectToAction("NotFound");
            }
            else
            {
                var result = await roleManager.DeleteAsync(role);
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
                    return RedirectToAction("listrole", "admin");
                }
            }
        }
        #endregion

        [HttpGet]
        public IActionResult ListUsers()
        {
            var result = userManager.Users;
            return View(result);
        }

        [HttpGet]
        public async Task<IActionResult> EditUser(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"role with id = {id} is not found";
                return RedirectToAction("NotFound");
            }
            else
            {
                var userRoles = await userManager.GetRolesAsync(user);
                var userClaims = await userManager.GetClaimsAsync(user);
                EditUserViewModel model = new EditUserViewModel
                {
                    Id = id,
                    UserName = user.UserName,
                    Email = user.Email,
                    City = user.City,
                    Claims = userClaims.Select(x => x.Type + " : " + x.Value).ToList(),
                    Roles = userRoles
                };
                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            var user = await userManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"role with id = {model.Id} is not found";
                return RedirectToAction("NotFound");
            }
            else
            {
                user.UserName = model.UserName;
                user.Email = model.Email;
                user.City = model.City;

                var result = await userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("listusers", "admin");
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
        public async Task<IActionResult> ManageUserRoles(string userId)
        {
            ViewBag.UserId = userId;
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"role with id = {userId} is not found";
                return RedirectToAction("NotFound");
            }
            else
            {
                ViewBag.UserName = user.UserName;
                var model = new List<UserRolesViewModel>();
                foreach (var role in roleManager.Roles)
                {
                    UserRolesViewModel userRoleViewModel = new UserRolesViewModel
                    {
                        RoleId = role.Id,
                        RoleName = role.Name
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
        public async Task<IActionResult> ManageUserRoles(List<UserRolesViewModel> model, string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"role with id = {userId} is not found";
                return RedirectToAction("NotFound", "home");
            }
            else
            {
                var roles = await userManager.GetRolesAsync(user);
                var result = await userManager.RemoveFromRolesAsync(user, roles);
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", "Cannot remove roles");
                    return View(model);
                }
                result = await userManager.AddToRolesAsync(user,
                    model.Where(x => x.IsSelected).Select(y => y.RoleName));
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", "Cannot add roles");
                    return View(model);
                }
                return RedirectToAction("EditUser", new { Id = userId });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ManageUserClaims(string userId)
        {
            ViewBag.UserId = userId;
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"role with id = {userId} is not found";
                return RedirectToAction("NotFound");
            }
            else
            {
                var model = new UserClaimsViewModel
                {
                    UserId = userId
                };
                var claims = await userManager.GetClaimsAsync(user);

                foreach (Claim claim in ClaimsStore.AllClaims)
                {
                    UserClaim userClaim = new UserClaim
                    {
                        ClaimType = claim.Type
                    };
                    if (claims.Any(x => x.Type == claim.Type && x.Value == "true"))
                    {
                        userClaim.IsSelected = true;
                    }
                    model.Claims.Add(userClaim);
                }
                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> ManageUserClaims(UserClaimsViewModel model)
        {
            var user = await userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"role with id = {model.UserId} is not found";
                return RedirectToAction("NotFound", "home");
            }
            else
            {
                var roles = await userManager.GetClaimsAsync(user);
                var result = await userManager.RemoveClaimsAsync(user, roles);
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", "Cannot remove roles");
                    return View(model);
                }
                result = await userManager.AddClaimsAsync(user,
                    model.Claims
                    .Select(y => new Claim(y.ClaimType, y.IsSelected ? "true" : "false")));
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", "Cannot add roles");
                    return View(model);
                }
                return RedirectToAction("EditUser", new { Id = model.UserId });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"role with id = {id} is not found";
                return RedirectToAction("NotFound");
            }
            else
            {
                var result = await userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("listusers", "admin");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View(result);
                }
            }
        }

    }
}
