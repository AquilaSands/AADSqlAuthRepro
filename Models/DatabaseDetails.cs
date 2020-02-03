using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace AADSqlAuthRepro.Models
{
    public class DatabaseDetails 
    {
        [HiddenInput]
        public bool SuccessfulConnection { get; set; }
        
        [Required]
        [Display(Name = "Server Address")]
        public string Server { get; set; }

        [Display(Name = "Port")]
        public int Port { get; set; } = 1433;

        [Required]
        [Display(Name = "Database Name")]
        public string DatabaseName { get; set; }

        [Required]
        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [HiddenInput]   
        public string ErrorMessage { get; set; }
    }
}