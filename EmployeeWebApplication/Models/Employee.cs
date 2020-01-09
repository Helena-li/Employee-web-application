using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeWebApplication.Models
{
    public class Employee
    {
        public int Id { get; set; }
        [NotMapped]
        public string EncryptedId { get; set; }
        [Required]
        [MaxLength(50,ErrorMessage ="Name cannot logger than 50 charactors")]
        public string Name { get; set; }
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9._+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z0-9-.]+$",
            ErrorMessage = "Invalid Email Format")] 
        public string Email { get; set; }
        public Dept Department { get; set; }
        public string PhotoPath { get; set; }
    }

    public enum Dept
    {
        None,
        IT,
        HR,
        RD
    }
}
