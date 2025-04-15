using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CollegeTrackAPI.Models;
using CollegeTrackAPI.Data;


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
    public async Task<ActionResult<IEnumerable<Track>>> GetTracks([FromQuery] TrackTypeEnum? trackType)
    {
        // Build the query to fetch tracks
        IQueryable<Track> query = _context.Tracks.Include(t => t.Courses);

        // If TrackType is provided, filter the tracks by TrackType
        if (trackType.HasValue)
        {
            query = query.Where(t => t.TrackType == trackType.Value);
        }

        // Execute the query and get the result
        var tracks = await query.ToListAsync();

        return Ok(tracks);
    }
}