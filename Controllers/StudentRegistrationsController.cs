using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CollegeTrackAPI.Models;
using CollegeTrackAPI.Data;

[ApiController]
[Route("api/[controller]")]
public class StudentRegistrationsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _env;

    public StudentRegistrationsController(AppDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    [HttpPost]
    public async Task<IActionResult> Register([FromForm] StudentRegistration model, IFormFile? resume, IFormFile? transcript, IFormFile? idCard)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        string folder = Path.Combine(_env.WebRootPath ?? "wwwroot", "uploads");
        Directory.CreateDirectory(folder);

        if (resume != null)
        {
            var fileName = $"{Guid.NewGuid()}_{resume.FileName}";
            var path = Path.Combine(folder, fileName);
            using var stream = new FileStream(path, FileMode.Create);
            await resume.CopyToAsync(stream);
            model.ResumePath = $"/uploads/{fileName}";
        }

        if (transcript != null)
        {
            var fileName = $"{Guid.NewGuid()}_{transcript.FileName}";
            var path = Path.Combine(folder, fileName);
            using var stream = new FileStream(path, FileMode.Create);
            await transcript.CopyToAsync(stream);
            model.TranscriptPath = $"/uploads/{fileName}";
        }

        if (idCard != null)
        {
            var fileName = $"{Guid.NewGuid()}_{idCard.FileName}";
            var path = Path.Combine(folder, fileName);
            using var stream = new FileStream(path, FileMode.Create);
            await idCard.CopyToAsync(stream);
            model.IdCardPath = $"/uploads/{fileName}";
        }

        _context.Registrations.Add(model);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Registration successful" });
    }
}
