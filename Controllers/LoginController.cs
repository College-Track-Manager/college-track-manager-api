using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using CollegeTrackAPI.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using CollegeTrackAPI.Services;

namespace CollegeTrackAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly IEmailSender _emailSender; // Add this line
        private readonly AuditService _auditService;

        public LoginController(AuditService auditService, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IConfiguration configuration, IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _emailSender = emailSender; // Assign it
            _auditService = auditService;

        }

        // POST api/login/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            // Check if the model is valid
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid model.");
            }

            // Try finding the user by username or email
            var user = await _userManager.FindByNameAsync(model.Username);
            if (user == null)
            {
                // If user is not found by username, try by email
                user = await _userManager.FindByEmailAsync(model.Username);
            }

            if (user == null)
            {
                // If user does not exist, return Unauthorized
                return Unauthorized(new { message = "User not found." });
            }

            // Check password
            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded)
            {
                // If password is incorrect, return Unauthorized
                return Unauthorized(new { message = "Invalid password." });
            }

            await _auditService.LogActionAsync(user.Email!, "Login", "Authentication", user.Id.ToString(), $"User {user.Email!} logged in");


            // Generate JWT Token
            var token = await GenerateJwtToken(user);
            return Ok(new { token });
        }

        private async Task<string> GenerateJwtToken(ApplicationUser user)
        {
            var username = user.UserName ?? "defaultUsername";
            var email = user.Email ?? "default@example.com";

            var roles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, username),
                    new Claim(JwtRegisteredClaimNames.Email, email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim("fullName", user.FullName ?? string.Empty),
                    new Claim("phone", user.Phone ?? string.Empty),
                    new Claim("address", user.Address ?? string.Empty)
                };

            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            // Read config with validation
            var secretKey = _configuration["Jwt:SecretKey"];
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];

            if (string.IsNullOrWhiteSpace(secretKey) || string.IsNullOrWhiteSpace(issuer) || string.IsNullOrWhiteSpace(audience))
                throw new InvalidOperationException("JWT configuration is missing.");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }



        [Authorize]
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            // In JWT, the "logout" action is simply the client removing the token from storage
            // This action could serve to notify the client to clear the token
            return Ok(new { message = "Successfully logged out. Please remove the token from storage." });
        }


        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return BadRequest("User not found");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetLink = $"http://localhost:5050/reset-password?email={model.Email}&token={Uri.EscapeDataString(token)}";

            await _emailSender.SendEmailAsync(
                model.Email,
                "Reset Your Password",
                $"Click <a href='{resetLink}'>here</a> to reset your password."
            );

            await _auditService.LogActionAsync(User, "ForgotPassword", "User", user.Id.ToString(), $"Password reset request for email: {model.Email}");

            return Ok("Password reset link sent.");
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Email))
                return BadRequest("Email is required");
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return BadRequest("User not found");

            if (string.IsNullOrWhiteSpace(model.Token) || string.IsNullOrWhiteSpace(model.NewPassword))
                return BadRequest("Token and new password are required.");
            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);

            if (!result.Succeeded)
                return BadRequest(result.Errors);
            else
            {
                await SendPasswordResetConfirmationAsync(model.Email);
                await _auditService.LogActionAsync(User, "ResetPassword", "User", user.Id.ToString(), $"Password reset for email: {model.Email}");

                return Ok("Password reset successful");
            }
        }

        private async Task SendPasswordResetConfirmationAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            var firstName = user?.FullName?.Split(' ')?.FirstOrDefault() ?? "User";

            var subject = "Your Password Has Been Changed";
            var body = $@"
        Dear {firstName},<br><br>
        This is a confirmation that your password has been successfully changed.<br><br>
        If you did not perform this action, please contact support immediately.<br><br>
        Best Regards,<br>
        College Track Team";

            await _emailSender.SendEmailAsync(email, subject, body);
        }

    }
}
