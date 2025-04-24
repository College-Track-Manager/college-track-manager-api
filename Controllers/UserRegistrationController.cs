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
            if (model.Password != model.ConfirmPassword)
                return BadRequest(new { message = "Passwords do not match" });

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FirstName + " " + model.LastName,
                NationalId = model.NationalId , 
                Address = model.Address ,
                Phone = model.Phone
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return BadRequest(new { message = "User creation failed", errors = result.Errors });

            // Assign "Student" role
            var roleResult = await _userManager.AddToRoleAsync(user, "Student");
            if (!roleResult.Succeeded)
                return BadRequest(new { message = "User created but failed to assign Student role", errors = roleResult.Errors });

            return Ok(new { message = "Registration successful with Student role" });
        }

    }
}
