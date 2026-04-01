using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WasteManagementSystem.Data;
using WasteManagementSystem.Models;

namespace WasteManagementSystem.Controllers
{
    public class HouseDetailsController : Controller
    {
        private readonly WasteContext _context;

        public HouseDetailsController(WasteContext context)
        {
            _context = context;
        }

        // GET: HouseDetails
        public async Task<IActionResult> Index()
        {
            return View(await _context.Houses.ToListAsync());
        }

        // GET: HouseDetails/Details/A65F4E2
        public async Task<IActionResult> Details(string id) // Changed to string
        {
            if (id == null) return NotFound();

            var houseDetails = await _context.Houses
                .FirstOrDefaultAsync(m => m.Eircode == id); // Changed from .Id

            if (houseDetails == null) return NotFound();

            return View(houseDetails);
        }

        // GET: HouseDetails/Edit/A65F4E2
        public async Task<IActionResult> Edit(string id) // Changed to string
        {
            if (id == null) return NotFound();

            var houseDetails = await _context.Houses.FindAsync(id);
            if (houseDetails == null) return NotFound();

            return View(houseDetails);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Address,Eircode,Password")] HouseDetails houseDetails)
        {
            if (id != houseDetails.Eircode) // Changed from .Id
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(houseDetails);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HouseDetailsExists(houseDetails.Eircode)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(houseDetails);
        }

        // GET: HouseDetails/Delete/A65F4E2
        public async Task<IActionResult> Delete(string id) // Changed to string
        {
            if (id == null) return NotFound();

            var houseDetails = await _context.Houses
                .FirstOrDefaultAsync(m => m.Eircode == id);

            if (houseDetails == null) return NotFound();

            return View(houseDetails);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id) // Changed to string
        {
            var houseDetails = await _context.Houses.FindAsync(id);
            if (houseDetails != null)
            {
                _context.Houses.Remove(houseDetails);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HouseDetailsExists(string id) // Changed to string
        {
            return _context.Houses.Any(e => e.Eircode == id);
        }
    }
}