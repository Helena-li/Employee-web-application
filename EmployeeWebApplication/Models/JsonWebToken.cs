using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeWebApplication.Models
{
    public class JsonWebToken
    {
        public string Token { get; set; }
        public long Expires { get; set; }
        public string UserRole { get; set; }
        public string Username { get; set; }
    }
}
