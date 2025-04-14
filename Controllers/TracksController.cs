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
    public async Task<ActionResult<IEnumerable<Track>>> GetTracks()
    {
        var tracks = await _context.Tracks
            .Include(t => t.Courses)
            .ToListAsync();

        return Ok(tracks);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Track>> GetTrackById(int id)
    {
        var track = await _context.Tracks
            .Include(t => t.Courses)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (track == null)
        {
            return NotFound();
        }

        return Ok(track);
    }
}