using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ClientManagement.Models
{
    [Keyless]
    public class SignInModel
    {
        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
