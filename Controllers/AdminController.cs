using System.Security.Claims;
using CollegeTrackAPI.Data;
using CollegeTrackAPI.Models;
using CollegeTrackAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CollegeTrackAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AuditService _auditService;
        private readonly CollegeTrackAPI.Services.IEmailSender _emailSender;

        public AdminController(CollegeTrackAPI.Services.IEmailSender emailSender,
            AuditService auditService, UserManager<ApplicationUser> userManager, AppDbContext context, IWebHostEnvironment env)
        {
            _userManager = userManager;
            _context = context;
            _env = env;
            _auditService = auditService;
            _emailSender = emailSender;

        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateAdminUser([FromForm] StudentRegistration model, IFormFile? resume, IFormFile? transcript, IFormFile? idCard)
        {

            return Ok(new { message = "Registration successful" });
        }
    }
}
