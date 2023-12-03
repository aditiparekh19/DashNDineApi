using Microsoft.AspNetCore.Identity;

namespace DashNDineApi.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
    }
}
