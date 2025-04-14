using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using CollegeTrackAPI.Models;
using Microsoft.Extensions.Configuration;

namespace CollegeTrackAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;

        public LoginController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
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

            // Generate JWT Token
            var token = GenerateJwtToken(user);
            return Ok(new { token });
        }

        private string GenerateJwtToken(ApplicationUser user)
        {
            var username = user.UserName ?? "defaultUsername";  // Provide default if null
            var email = user.Email ?? "default@example.com";    // Provide default if null

            // Define claims for JWT
            var claims = new[] 
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())  // Unique identifier
            };

            var secretKey = _configuration["Jwt:SecretKey"];
            if (string.IsNullOrEmpty(secretKey))
            {
                // Handle missing secret key more gracefully
                return "JWT Secret Key is not configured.";
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Generate JWT token with expiration time of 1 day
            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
            );

            // Return the token
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // POST api/login/logout
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            // In JWT, the "logout" action is simply the client removing the token from storage
            // This action could serve to notify the client to clear the token
            return Ok(new { message = "Successfully logged out. Please remove the token from storage." });
        }
    }
}
