using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Laundry.Models
{
    public class LoginUser
    {
        [Required]
        [EmailAddress]
        public string Email {get;set;}
        [DataType(DataType.Password)]
        [Required]
        [MinLength(8,ErrorMessage="Password must be 8 characters or longer!")]
        public string Password {get;set;}
    }
}  