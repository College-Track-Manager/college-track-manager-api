using Microsoft.AspNetCore.Identity;

namespace CollegeTrackAPI.Models
{
    public class ApplicationUser : IdentityUser
    {
        // Add this constructor to initialize FullName
        public ApplicationUser()
        {
            FullName = string.Empty; // Or provide a default value
        }

        public string FullName { get; set; } // Make sure it's initialized
    }

}