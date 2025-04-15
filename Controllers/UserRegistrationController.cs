using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using CollegeTrackAPI.Models; // Ensure ApplicationUser is imported
using System.Threading.Tasks;

namespace CollegeTrackAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserRegistrationController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserRegistrationController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            // Check if the passwords match
            if (model.Password != model.ConfirmPassword)
                return BadRequest(new { message = "Passwords do not match" });

            // Create the user object (no validation for email, phone, etc.)
            var user = new ApplicationUser
            {
                UserName = model.Email, // Assuming Email is used as the username
                Email = model.Email,
                FullName = model.FirstName + " " + model.LastName,  // Assuming FullName is a custom property in ApplicationUser
                NationalId = model.NationalId // Save NationalId here
            };

            // Create the user with hashed password
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return BadRequest(new { message = "User creation failed", errors = result.Errors });

            return Ok(new { message = "Registration successful" });
        }
    }
}
