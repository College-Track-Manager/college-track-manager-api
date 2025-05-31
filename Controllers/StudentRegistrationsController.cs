using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CollegeTrackAPI.Models;
using CollegeTrackAPI.Data;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using CollegeTrackAPI.Services;
using Microsoft.AspNetCore.Identity.UI.Services;

[ApiController]
[Route("api/[controller]")]
public class StudentRegistrationsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _env;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly AuditService _auditService;
    private readonly CollegeTrackAPI.Services.IEmailSender _emailSender;



    public StudentRegistrationsController(CollegeTrackAPI.Services.IEmailSender emailSender,
        AuditService auditService, UserManager<ApplicationUser> userManager, AppDbContext context, IWebHostEnvironment env)
    {
        _userManager = userManager;
        _context = context;
        _env = env;
        _auditService = auditService;
        _emailSender = emailSender;


    }

    [Authorize(Roles = "Student")]
    [HttpPost]
    public async Task<IActionResult> Register([FromForm] StudentRegistration model, IFormFile? resume, IFormFile? transcript, IFormFile? idCard)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var email = User.FindFirst(ClaimTypes.Email)?.Value;

        var currentUserEmail = User.Identity?.Name;
        if (string.IsNullOrEmpty(currentUserEmail))
            return Unauthorized(new { message = "Invalid user context" });

        // Assign student email to model (as owner)
        model.Email = currentUserEmail;

        // Check if already registered this academic year
        var existingRegistration = await _context.Registrations
            .FirstOrDefaultAsync(r => r.Email == currentUserEmail && r.AcademicYear == model.AcademicYear);

        if (existingRegistration != null)
            return BadRequest(new { message = "You have already registered for a track this academic year." });

        string secureFolder = Path.Combine(_env.ContentRootPath, "SecureFiles", "uploads");
        Directory.CreateDirectory(secureFolder);

        async Task<string?> SaveFileAsync(IFormFile? file)
        {
            if (file == null) return null;

            var fileName = $"{Guid.NewGuid()}_{file.FileName}";
            var path = Path.Combine(secureFolder, fileName);
            using var stream = new FileStream(path, FileMode.Create);
            await file.CopyToAsync(stream);
            return fileName; // Save filename only, no full URL
        }
        if (resume != null)
            model.ResumePath = await SaveFileAsync(resume);

        if (transcript != null)
            model.TranscriptPath = await SaveFileAsync(transcript);

        if (idCard != null)
            model.IdCardPath = await SaveFileAsync(idCard);

        // Look up the track based on TrackId
        var track = await _context.Tracks.FirstOrDefaultAsync(t => t.Id == model.TrackId);
        if (track == null)
            return BadRequest(new { message = "Invalid track ID." });

        model.Track = track;  // Assign the track to the registration model

        _context.Registrations.Add(model);
        await _context.SaveChangesAsync();

        var user = await _userManager.FindByEmailAsync(currentUserEmail);
        var fullName = user?.FullName ?? "User";

        // Send email to user after registration with the track name
        await SendRegistrationEmailAsync(currentUserEmail, track.Title, fullName);

        await _auditService.LogActionAsync(User, "Create", "StudentRegistrations", model.Id.ToString(), $"Student {model.Email} registered for track {track.Title}");

        return Ok(new { message = "Registration successful" });
    }


    [HttpGet("{email}")]
    [Authorize(Roles = "Student,Admin")]
    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        // Get current user's email from token
        var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
        if (string.IsNullOrEmpty(userEmail))
            return Unauthorized("Email not found in token.");

        // Fetch registration record using email
        var student = await _context.Registrations
            .Include(s => s.Track)
                .ThenInclude(t => t.TrackCourses)
                    .ThenInclude(tc => tc.Course)
            .FirstOrDefaultAsync(s => s.Email == userEmail);

        if (student == null)
            return NotFound(new { message = "Student registration not found." });

        // Fetch user profile from AspNetUsers
        var user = await _userManager.FindByEmailAsync(userEmail);
        if (user == null)
            return NotFound(new { message = "User profile not found." });

        await _auditService.LogActionAsync(User, "Read", "Profile", user.Id, $"Viewed profile of user {user.Email}");


        return Ok(new
        {
            FullName = user.FullName,
            Email = user.Email,
            Phone = user.Phone,
            Address = user.Address,
            student.TrackDegree,
            student.TrackType,
            student.StudyType,
            student.Education,
            student.Statement,
            student.RegistrationDate,
            Track = new
            {
                student.Track.Id,
                student.Track.Title,
                student.Track.ShortDescription,
                student.Track.Duration
            },
            Courses = student.Track.TrackCourses.Select(tc => new
            {
                tc.Course.CourseCode,
                tc.Course.Title,
                tc.Course.Description,
                tc.Course.Credits
            })
        });
    }

    // Send Registration Email after successful registration
    private async Task SendRegistrationEmailAsync(string email, string trackName, string fullName)
    {
        var subject = "Track Registration Successful";
        var body = $@"
        Dear {fullName},<br><br>
        You have successfully registered for the track <b>{trackName}</b>.<br>
        We are excited to have you on board!<br><br>
        Best Regards,<br>
        College Track Team";

        await _emailSender.SendEmailAsync(email, subject, body);
    }


}
