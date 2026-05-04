using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WasteManagementSystem.Services;
using WasteManagementSystem.Data;

namespace WasteManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WasteStatsController : ControllerBase
    {
        private readonly WasteContext _context;

        public WasteStatsController(WasteContext context)
        {
            _context = context;
        }

        // GET: api/wastestats/summary
        // provides an overview of the entire system
        [HttpGet("summary")]
        public async Task<IActionResult> GetGlobalSummary()
        {
            var stats = new
            {
                SystemStatus = "Active",
                Timestamp = DateTime.Now,
                TotalHouseholds = await _context.Houses.CountAsync(),
                TotalItemsInSystem = await _context.Items.CountAsync(),
                TotalWasteEvents = await _context.WasteLogs.CountAsync()
            };

            return Ok(stats);
        }

        // GET: api/wastestats/household/D02X285
        // provides specific data to a household
        [HttpGet("household/{eircode}")]
        public async Task<IActionResult> GetHouseholdStats(string eircode)
        {
            var house = await _context.Houses
                .Include(h => h.Items)
                .FirstOrDefaultAsync(h => h.Eircode == eircode);

            if (house == null)
            {
                return NotFound(new { Message = $"Eircode: {eircode} not found." });
            }

            return Ok(new
            {
                house.Eircode,
                ItemCount = house.Items?.Count ?? 0,
                Items = house.Items?.Select(i => new { i.ItemName, i.Value })
            });
        }


        [HttpGet("map-data")]
        public async Task<IActionResult> GetMapData()
        {
            var dbData = await _context.Houses
                .Select(h => new { Prefix = h.Eircode.Substring(0, 3) })
                .GroupBy(x => x.Prefix)
                .Select(g => new { Area = g.Key, Count = g.Count() })
                .ToListAsync();

            var result = dbData
                .Where(d => MapLocationServices.AreaCoordinates.ContainsKey(d.Area)) // filter to only include areas we have coordinates for
                .Select(d => new {
                    d.Area,
                    d.Count,
                    Coords = new
                    {
                        Lat = MapLocationServices.AreaCoordinates[d.Area].Lat,
                        Lng = MapLocationServices.AreaCoordinates[d.Area].Lng
                    }
                });

            return Ok(result);
        }

    }
}
