using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OnSale.Web.Models
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Username { get; set; }

        [Required]
        [MinLength(6,ErrorMessage = "no more than 6 characters allowed")]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }

}
