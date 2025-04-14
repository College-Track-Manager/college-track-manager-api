using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using CollegeTrackAPI.Models;  // Ensure ApplicationUser is imported

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
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = new ApplicationUser
            {
                UserName = model.Username,
                Email = model.Email,
                FullName = model.FullName // Assuming FullName is a custom property in ApplicationUser
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
                return Ok(new { message = "Registration successful" });

            return BadRequest(result.Errors);
        }
    }
}
