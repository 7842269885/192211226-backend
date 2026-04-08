using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GrowsmartAPI.Data;
using GrowsmartAPI.Models;

namespace GrowsmartAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PlantsController : ControllerBase
{
    private readonly AppDbContext _context;

    public PlantsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<Plant>>> GetUserPlants(int userId)
    {
        return await _context.Plants.Where(p => p.UserId == userId).ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Plant>> GetPlant(int id)
    {
        var plant = await _context.Plants.FindAsync(id);

        if (plant == null)
            return NotFound();

        return plant;
    }

    [HttpPost]
    public async Task<ActionResult<Plant>> PostPlant([FromBody] Plant plant)
    {
        try 
        {
            // Debug Log the incoming object
            Console.WriteLine($"[DEBUG] Received Plant: Name={plant.Name}, UserId={plant.UserId}, Category={plant.Category}, IsIndoor={plant.IsIndoor}");

            if (plant.UserId <= 0) 
            {
               Console.WriteLine($"[DEBUG] REJECTED: UserId is {plant.UserId}");
               return BadRequest("Valid UserId is required.");
            }

            var userExists = await _context.Users.AnyAsync(u => u.Id == plant.UserId);
            if (!userExists)
            {
                Console.WriteLine($"[DEBUG] REJECTED: User {plant.UserId} not found in DB.");
                return BadRequest($"Invalid UserId: User {plant.UserId} does not exist in the database.");
            }

            _context.Plants.Add(plant);
            await _context.SaveChangesAsync();
            Console.WriteLine($"[DEBUG] SUCCESS: Plant saved with ID {plant.Id}");
            return CreatedAtAction(nameof(GetPlant), new { id = plant.Id }, plant);
        }
        catch (Exception ex)
        {
            var innerMessage = ex.InnerException?.Message ?? ex.Message;
            Console.WriteLine($"[ERROR] Failed to add plant: {ex}"); // Full log in terminal
            return StatusCode(500, $"DB Error: {innerMessage}. Source: {ex.Source}");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutPlant(int id, Plant plant)
    {
        if (id != plant.Id)
            return BadRequest();

        _context.Entry(plant).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!PlantExists(id))
                return NotFound();
            throw;
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePlant(int id)
    {
        var plant = await _context.Plants.FindAsync(id);
        if (plant == null)
            return NotFound();

        _context.Plants.Remove(plant);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool PlantExists(int id)
    {
        return _context.Plants.Any(e => e.Id == id);
    }
}
