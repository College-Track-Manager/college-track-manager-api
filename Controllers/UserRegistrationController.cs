using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using CollegeTrackAPI.Models; // Ensure ApplicationUser is imported
using System.Threading.Tasks;
using CollegeTrackAPI.Services;

namespace CollegeTrackAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserRegistrationController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly CollegeTrackAPI.Services.IEmailSender _emailSender;

        public UserRegistrationController(UserManager<ApplicationUser> userManager, CollegeTrackAPI.Services.IEmailSender emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (model.Password != model.ConfirmPassword)
                return BadRequest(new { message = "Passwords do not match" });

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = $"{model.FirstName ?? ""} {model.LastName ?? ""}",
                NationalId = model.NationalId,
                Address = model.Address,
                Phone = model.Phone
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return BadRequest(new { message = "User creation failed", errors = result.Errors });

            var roleResult = await _userManager.AddToRoleAsync(user, "Student");
            if (!roleResult.Succeeded)
                return BadRequest(new { message = "User created but failed to assign Student role", errors = roleResult.Errors });


            var subject = "Track Registration Successful";
            var body = $@"
        Dear {user.UserName},<br><br>
        Welcomwe to  <b>College of Graduate Studies and Statistical Research</b>.<br>
We are excited to welcome you to our community!<br><br>
        We are excited to have you on board!<br><br>
        Best Regards,<br>
        College Track Team";

            await _emailSender.SendEmailAsync(user.Email, subject, body);

            return Ok(new { message = "Registration successful with Student role" });
        }


    }
}
