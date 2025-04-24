using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CollegeTrackAPI.Data;
using CollegeTrackAPI.Models;
using CollegeTrackAPI.DTOs;
using CollegeTrackAPI.Services;

[Route("api/[controller]")]
[ApiController]
public class CoursesController : ControllerBase
{
    private readonly AppDbContext _context;

    private readonly AuditService _auditService;

    public CoursesController(AppDbContext context, AuditService auditService)
    {
        _context = context;
        _auditService = auditService;
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> AddCourse([FromBody] CourseCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var course = new Course
        {
            CourseCode = dto.CourseCode,
            Title = dto.Title,
            Description = dto.Description,
            Credits = dto.Credits
        };

        _context.Courses.Add(course);
        await _context.SaveChangesAsync();

        await _auditService.LogActionAsync(User, "Create", "Courses", course.Id.ToString(), $"Course {course.Title} added");

        return Ok(new { message = "Course added successfully", course.Id });
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCourse(int id, [FromBody] CourseCreateDto dto)
    {
        var course = await _context.Courses.FindAsync(id);
        if (course == null)
            return NotFound(new { message = "Course not found" });

        course.CourseCode = dto.CourseCode;
        course.Title = dto.Title;
        course.Description = dto.Description;
        course.Credits = dto.Credits;

        await _context.SaveChangesAsync();

        await _auditService.LogActionAsync(User, "Update", "Courses", id.ToString(), $"Updated course {course.Title}");

        return Ok(new { message = "Course updated successfully" });
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCourse(int id)
    {
        var course = await _context.Courses
            .Include(c => c.TrackCourses)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (course == null)
            return NotFound(new { message = "Course not found" });

        if (course.TrackCourses.Any())
            return BadRequest(new { message = "Cannot delete course because it is assigned to one or more tracks." });

        _context.Courses.Remove(course);
        await _context.SaveChangesAsync();

        await _auditService.LogActionAsync(User, "Delete", "Courses", id.ToString(), $"Deleted course {course.Title}");

        return Ok(new { message = "Course deleted successfully" });
    }

    [HttpGet]
    public async Task<IActionResult> GetAllCourses([FromQuery] string? courseCode, [FromQuery] string? title, [FromQuery] int? credits)
    {
        var query = _context.Courses.AsQueryable();

        if (!string.IsNullOrWhiteSpace(courseCode))
            query = query.Where(c => c.CourseCode.Contains(courseCode));

        if (!string.IsNullOrWhiteSpace(title))
            query = query.Where(c => c.Title.Contains(title));

        if (credits.HasValue)
            query = query.Where(c => c.Credits == credits.Value);

        var courses = await query.ToListAsync();
        return Ok(courses);
    }

 
}
