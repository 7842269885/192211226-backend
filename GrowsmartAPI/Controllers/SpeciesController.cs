using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GrowsmartAPI.Data;
using GrowsmartAPI.Models;

namespace GrowsmartAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SpeciesController : ControllerBase
{
    private readonly AppDbContext _context;

    public SpeciesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<PlantSpecies>>> GetAll()
    {
        return await _context.Species.ToListAsync();
    }

    [HttpGet("search")]
    public async Task<ActionResult<List<PlantSpecies>>> Search([FromQuery] string query)
    {
        if (string.IsNullOrEmpty(query)) return await GetAll();

        return await _context.Species
            .Where(s => s.CommonName.Contains(query) || s.ScientificName.Contains(query))
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PlantSpecies>> GetById(int id)
    {
        var species = await _context.Species.FindAsync(id);
        if (species == null) return NotFound();
        return species;
    }
}
