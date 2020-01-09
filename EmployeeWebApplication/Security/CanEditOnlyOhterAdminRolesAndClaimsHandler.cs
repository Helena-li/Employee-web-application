using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EmployeeWebApplication.Security
{
    public class CanEditOnlyOhterAdminRolesAndClaimsHandler
        : AuthorizationHandler<ManageAdminRolesAndClaimsRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            ManageAdminRolesAndClaimsRequirement requirement)
        {
            var authFilterContext = context.Resource as AuthorizationFilterContext;
            if (authFilterContext == null)
            {
                return Task.CompletedTask;
            }
            string loggedInAdminId = context.User.Claims.FirstOrDefault(c =>
              c.Type == ClaimTypes.NameIdentifier).Value;
            string adminInBeingEdit = authFilterContext.HttpContext.Request.Query["userId"];
            if (context.User.IsInRole("admin") &&
                context.User.HasClaim(x => x.Type == "Edit Role" && x.Value == "true") &&
                loggedInAdminId.ToLower() != adminInBeingEdit.ToLower())
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}
