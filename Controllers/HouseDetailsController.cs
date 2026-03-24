using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        // GET: HouseDetails/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var houseDetails = await _context.Houses
                .FirstOrDefaultAsync(m => m.Id == id);
            if (houseDetails == null)
            {
                return NotFound();
            }

            return View(houseDetails);
        }

        // GET: HouseDetails/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: HouseDetails/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Address,Eircode,Email,Password")] HouseDetails houseDetails)
        {
            if (ModelState.IsValid)
            {
                _context.Add(houseDetails);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(houseDetails);
        }

        // GET: HouseDetails/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var houseDetails = await _context.Houses.FindAsync(id);
            if (houseDetails == null)
            {
                return NotFound();
            }
            return View(houseDetails);
        }

        // POST: HouseDetails/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Address,Eircode,Email,Password")] HouseDetails houseDetails)
        {
            if (id != houseDetails.Id)
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
                    if (!HouseDetailsExists(houseDetails.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(houseDetails);
        }

        // GET: HouseDetails/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var houseDetails = await _context.Houses
                .FirstOrDefaultAsync(m => m.Id == id);
            if (houseDetails == null)
            {
                return NotFound();
            }

            return View(houseDetails);
        }

        // POST: HouseDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var houseDetails = await _context.Houses.FindAsync(id);
            if (houseDetails != null)
            {
                _context.Houses.Remove(houseDetails);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HouseDetailsExists(int id)
        {
            return _context.Houses.Any(e => e.Id == id);
        }
    }
}
