using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTAuthorizationWithRefreshToken.DbService.Models
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "User Name is required field")]
        public string? UserName { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "Email is required and of type email")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }
    }
}
