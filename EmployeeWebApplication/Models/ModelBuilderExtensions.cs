using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeWebApplication.Models
{
    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>().HasData(
                new Employee
                {
                    Id = 1,
                    Name = "Mary",
                    Department = Dept.HR,
                    Email = "mary@aaa.com"
                },
                new Employee
                {
                    Id = 2,
                    Name = "Jon",
                    Department = Dept.HR,
                    Email = "jon@aaa.com"
                }
                );
        }
    }
}
