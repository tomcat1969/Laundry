using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace Laundry.Models
{
    public class User
    {
        [Key]
        public int UserId {get;set;}

        [Required]
        [MinLength(2)]
         public string FirstName { get; set; }

        [Required]
        [MinLength(2)]
         public string LastName { get; set; }

         [EmailAddress]
         [Required]
         public string Email { get; set; }

         [DataType(DataType.Password)]
         [Required]
         [MinLength(8,ErrorMessage="Password must be 8 characters or longer!")]
         public string Password { get; set; }

         public bool IsAdmin {get;set;}
            // The MySQL DATETIME type can be represented by a DateTime
         public DateTime CreatedAt {get;set;} = DateTime.Now;
         public DateTime UpdatedAt {get;set;} = DateTime.Now;

         // Navigation property for related Message objects
         public List<WashingMachine> WashingMachinesInUse {get;set;}

         [NotMapped]
         [Compare("Password")]
         [DataType(DataType.Password)]
         public string Confirm{get;set;}
       
    }
}