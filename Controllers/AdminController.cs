using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WasteManagementSystem.Data;

namespace WasteManagementSystem.Controllers
{
    [Authorize(Roles = "Admin")] 
    public class AdminController : Controller
    {
        private readonly WasteContext _context;

        public AdminController(WasteContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var households = await _context.Houses
                .Include(h => h.Items)
                .ToListAsync();

            return View(households);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var house = await _context.Houses.FindAsync(id);
            if (house != null)
            {
                _context.Houses.Remove(house);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}