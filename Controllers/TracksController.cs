using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CollegeTrackAPI.Models;
using CollegeTrackAPI.Data;
using CollegeTrackAPI.DTOs;

[ApiController]
[Route("api/[controller]")]
public class TracksController : ControllerBase
{
    private readonly AppDbContext _context;

    public TracksController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> GetTracks([FromQuery] TrackTypeEnum? trackType)
    {
        var query = _context.Tracks
            .Include(t => t.TrackCourses)
                .ThenInclude(tc => tc.Course)
            .AsQueryable();

        if (trackType.HasValue)
        {
            query = query.Where(t => t.TrackType == trackType.Value);
        }

        var result = await query.Select(t => new
        {
            t.Id,
            t.Title,
            t.TitleEn,
            t.TrackType,
            t.TrackDegree,
            t.ShortDescription,
            t.FullDescription,
            t.Duration,
            t.CareerOutlook,
            t.Image,
            Requirements = t.Requirements,
            Courses = t.TrackCourses.Select(tc => new
            {
                tc.Course.Id,
                tc.Course.CourseCode,
                tc.Course.Title,
                tc.Course.Description,
                tc.Course.Credits
            }).ToList()
        }).ToListAsync();

        return Ok(result);
    }


    [HttpPost]
    public async Task<IActionResult> CreateTrack([FromBody] TrackCreateDto dto)
    {
        var courses = await _context.Courses
            .Where(c => dto.CourseIds.Contains(c.Id))
            .ToListAsync();

        if (courses.Count != dto.CourseIds.Count)
            return BadRequest("One or more course IDs are invalid.");

        var track = new Track
        {
            Title = dto.Title,
            TitleEn = dto.TitleEn,
            TrackType = dto.TrackType,
            TrackDegree = dto.TrackDegree,
            ShortDescription = dto.ShortDescription,
            FullDescription = dto.FullDescription,
            Duration = dto.Duration,
            CareerOutlook = dto.CareerOutlook,
            Image = dto.Image,
            Requirements = dto.Requirements,
            TrackCourses = courses.Select(c => new TrackCourse { CourseId = c.Id }).ToList()
        };

        _context.Tracks.Add(track);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetTrackById), new { id = track.Id }, new
        {
            track.Id,
            track.Title,
            track.TitleEn,
            track.TrackType,
            track.TrackDegree,
            track.ShortDescription,
            track.FullDescription,
            track.Duration,
            track.CareerOutlook,
            track.Image,
            track.Requirements,
            Courses = courses.Select(c => new
            {
                c.Id,
                c.Title,
                c.Description,
                c.CourseCode,
                c.Credits
            }).ToList()
        });
    }



    [HttpGet("{id}")]
    public async Task<IActionResult> GetTrackById(int id)
    {
        var track = await _context.Tracks
            .Include(t => t.TrackCourses)
                .ThenInclude(tc => tc.Course)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (track == null)
            return NotFound();

        var response = new
        {
            track.Id,
            track.Title,
            track.TitleEn,
            track.TrackType,
            track.TrackDegree,
            track.ShortDescription,
            track.FullDescription,
            track.Duration,
            track.CareerOutlook,
            track.Image,
            track.Requirements,
            Courses = track.TrackCourses.Select(tc => new
            {
                tc.Course.Id,
                tc.Course.CourseCode,
                tc.Course.Title,
                tc.Course.Description,
                tc.Course.Credits
            }).ToList()
        };

        return Ok(response);
    }



}
