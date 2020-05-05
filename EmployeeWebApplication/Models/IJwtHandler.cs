using Microsoft.IdentityModel.JsonWebTokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EmployeeWebApplication.Models
{
    public interface IJwtHandler
    {
        JsonWebToken Create(string userId, string userRole, bool isSignUp);
        ClaimsPrincipal ValidateToken(string token);
    }
}
