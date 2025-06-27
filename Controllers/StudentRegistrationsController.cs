using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CollegeTrackAPI.Models;
using CollegeTrackAPI.Data;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using CollegeTrackAPI.Services;
using Microsoft.AspNetCore.Identity.UI.Services;
using CollegeTrackAPI.DTOs;

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
            .FirstOrDefaultAsync(r => r.Email == currentUserEmail && r.AcademicYear == model.AcademicYear && r.Status != 2);

        if (existingRegistration != null)
            return BadRequest(new
            {
                error = "AlreadyRegistered",
                message = "You have already registered for a track this academic year."
            });

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
    
    [Authorize(Roles = "Student,Admin")]
    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        try
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
                student.Status,
                student.AdminComments,
                StatusDesctiption = student.Status switch // Convert integer to string here
                {
                    0 => "قيد الدراسة",
                    1 => "تم الموافقة",
                    2 => "مرفوض",
                    -1 => "لم يحدد" // Fallback for unexpected values
                },
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
        catch (Exception ex)
        {

            throw;
        }
    }

    
    [HttpGet("GetStudentRegistratrions")]
    public async Task<ActionResult<IEnumerable<object>>> GetStudentRegistratrions([FromQuery] StudentRegistrationStatus? studentRegistrationType)
    {
        var query = _context.Registrations
            .Include(t => t.Track)
            .AsQueryable();

        if (studentRegistrationType.HasValue)
        {
            if (studentRegistrationType == StudentRegistrationStatus.Pending)
            {
                query = query.Where(t => t.Status == (int)studentRegistrationType.Value);
            }
            else if (studentRegistrationType == StudentRegistrationStatus.Processed)
            {
                query = query.Where(t => t.Status != (int)StudentRegistrationStatus.Pending);
            }            
        }

        var studentRegistrationResult = await query.ToListAsync();      
        
        studentRegistrationResult.ForEach(x => x.Name = _userManager.FindByEmailAsync(x.Email)?.Result?.FullName);

        List<StudentRegistrationDto> studentRegistrations = new List<StudentRegistrationDto>();

        foreach (var item in studentRegistrationResult)
        {
            StudentRegistrationDto s = new StudentRegistrationDto
            {             
                Id = item.Id,             
                Name = item.Name,                
                Email = item.Email,              
                Track = item.Track.Title,
                RegistrationDate = item.RegistrationDate.ToShortDateString(),
                Education = item.Education,
                Statement = item.Statement,
                ResumePath = item.ResumePath,
                TranscriptPath = item.TranscriptPath,
                IdCardPath = item.IdCardPath,             
                Status = item.Status
            };
            studentRegistrations.Add(s);
        }
        

        return Ok(studentRegistrations);
    }


    [HttpGet("GetMyRegistrations")]
    public async Task<ActionResult<IEnumerable<object>>> GetMyRegistrations([FromQuery] string nationalId)
    {
        try
        {
            var query = _context.Registrations
            .AsQueryable();
         
            if (String.IsNullOrEmpty(nationalId))
            {
                return Ok("الرجاء ادخل رقم صحيح");
            }

            var users = _context.Users.Where( u => u.NationalId == nationalId.ToString()).FirstOrDefaultAsync();

            var email = users.Result?.Email;

            query = query.Where(t => t.Email == email);

            var result = await query.ToListAsync();

            if (result?.Count > 0)
                return Ok(result);
            else
                return Ok(" لا توجد بيانات للرقم القومي المدخل");
        }
        catch (Exception)
        {
            return BadRequest();
        }
    }
    [HttpGet("GetRegistrationById")]
    public async Task<ActionResult<IEnumerable<object>>> GetRegistrationById([FromQuery] int id)
    {
        try
        {
            if (id <= 0)
            {
                return Ok("الرجاء ادخل رقم صحيح");
            }

            var query = _context.Registrations.Where(u => u.Id == id).Include(s => s.Track)
                    .ThenInclude(t => t.TrackCourses)
                        .ThenInclude(tc => tc.Course)
                .FirstOrDefaultAsync();

            var user = _context.Users.Where(u => u.Email == query.Result.Email).FirstOrDefault();


            ApplicationDataDto application = new ApplicationDataDto();

            application.Name = user.FullName;
            application.SubmissionDate = query.Result.RegistrationDate.ToString();
            application.TranscriptUrl = query.Result.TranscriptPath;
            application.ResumeUrl = query.Result.ResumePath;
            application.IdCardUrl = query.Result.IdCardPath;
            application.EducationLevel = query.Result.Education;
            application.Education = query.Result.Education;
            application.StudyType = query.Result.StudyType.ToString();
            application.Track = query.Result.Track.Title;
            application.Email = user.Email;
            application.TrackDegree = (int)query.Result.TrackDegree == 1 ? "دبلومة": 
                                      (int)query.Result.TrackDegree == 2 ? "ماجسستير" :
                                      "دكتوراة";




            if (query != null)
                return Ok(application);
            else
                return Ok(" لا يوجد بيانات لرقم التسجيل ");
        }
        catch (Exception)
        {
            return BadRequest();
        }
    }
    //[Authorize(Roles = "Admin")]
    [HttpPut("UpdateStudentRegistration")]
    public async Task<ActionResult<StudentRegistration>> UpdateStudentRegistration(int id,int status,string? comments)
    {
        if (id <= 0)
        {
            return Ok("الرجاء ادخل رقم صحيح");
        }
      
        var registration = await _context.Registrations.FindAsync(id);

        if (registration == null)
        {
            return NotFound(new { message = "لم يتم العثور على التسجيل المطلوب" });
        }

        // Update only Status and Comments
        registration.Status = status;
        registration.AdminComments = comments;

        // Mark as modified (optional - EF usually detects changes automatically)
        _context.Entry(registration).Property(x => x.Status).IsModified = true;
        _context.Entry(registration).Property(x => x.AdminComments).IsModified = true;

        await _context.SaveChangesAsync();

        await _auditService.LogActionAsync(
            User,
            "UpdateStatus",
            "Registrations",
            id.ToString(),
            $"Updated status to {registration.Status} and comments to {registration.AdminComments} for student registration {id}"
        );

        // Send email to student based on new status
        var user = await _userManager.FindByEmailAsync(registration.Email);
        var fullName = user?.FullName ?? "Student";

        if (status == 1)
        {
            await SendStatusUpdateEmailAsync(registration.Email, fullName, "approved", registration.AdminComments);
        }
        else if (status == 2)
        {
            await SendStatusUpdateEmailAsync(registration.Email, fullName, "rejected", registration.AdminComments);
        }

        return Ok(new { message = "تم تحديث حالة التسجيل بنجاح" });
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

    private async Task SendStatusUpdateEmailAsync(string email, string fullName, string status, string comments)
    {
        var subject = $"Track Registration {status.ToUpper()}";
        var statusMessage = status == "approved" ? "has been <b>approved</b>" : "has been <b>rejected</b>";
        var body = $@"
        Dear {fullName},<br><br>
        Your track registration {statusMessage}.<br>
        <b>Admin Comments:</b> {comments}<br><br>
        Best regards,<br>
        College Track Team";

        await _emailSender.SendEmailAsync(email, subject, body);
    }
}
