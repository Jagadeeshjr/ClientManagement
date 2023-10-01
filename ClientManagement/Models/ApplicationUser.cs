using Microsoft.AspNetCore.Identity;

namespace ClientManagement.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }

        public string SecondName { get; set; }

    }
}
