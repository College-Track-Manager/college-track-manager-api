using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using CollegeTrackAPI.Data; // Make sure the namespace is correct
using CollegeTrackAPI.Services;

[Authorize] // Or use [Authorize(Roles = "Admin,Student")]
[ApiController]
[Route("api/[controller]")]
public class SecureFilesController : ControllerBase
{
    private readonly IWebHostEnvironment _env;
    private readonly AppDbContext _context;
    private readonly AuditService _auditService;

    public SecureFilesController(AuditService auditService ,IWebHostEnvironment env, AppDbContext context)
    {
        _env = env;
        _context = context;
        _auditService = auditService;
    }

    [Authorize]
    [HttpGet("download/{fileName}")]
    public async Task<IActionResult> GetFile(string fileName)
    {
        var folderPath = Path.Combine(_env.ContentRootPath, "SecureFiles", "uploads");
        var fullPath = Path.Combine(folderPath, fileName);

        if (!System.IO.File.Exists(fullPath))
            return NotFound("File not found.");

        // Get current user email from token
        var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;

        if (string.IsNullOrEmpty(userEmail))
            return Unauthorized("Missing email in token.");

        // Get user role from token
        var roles = User.FindAll(ClaimTypes.Role).Select(r => r.Value);
        bool isAdmin = roles.Contains("Admin");

        // Only admins or the student who owns the file can access it
        // Check if any registration belongs to this student and has this file
        var registration = await _context.Registrations
            .FirstOrDefaultAsync(r =>
                (   r.ResumePath == fileName ||
                    r.TranscriptPath == fileName ||
                    r.IdCardPath == fileName) &&
                r.Email == userEmail);

        if (registration == null && !isAdmin)
            return StatusCode(403, new { message = "You are not authorized to access this file." });

        await _auditService.LogActionAsync(User, "Read", "SecureFiles", fileName, $"Downloaded file {fileName}");

        // Return the file
        var contentType = "application/octet-stream";
        var fileBytes = await System.IO.File.ReadAllBytesAsync(fullPath);
        return File(fileBytes, contentType, fileName);
    }

}

