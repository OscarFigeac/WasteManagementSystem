using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

    }
}
