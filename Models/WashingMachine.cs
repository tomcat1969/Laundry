using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Laundry.Models
{
    public class WashingMachine
    {
        [Key]
        public int WashingMachineId {get;set;}

        [Required]
        public String ModelName {get;set;}

        [Required]
        public String PicURL {get;set;}

        public string status {get;set;}

       

        public int UserId {get;set;}
        
        public User UserOccupied{get;set;}

        public DateTime UsedAt {get;set;}

        public int Duration {get;set;}
        public int DurationLeft {get;set;}




        

        public DateTime CreatedAt {get;set;} = DateTime.Now;
        public DateTime UpdatedAt {get;set;} = DateTime.Now;



    }
}  