using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WasteManagementSystem.Data;
using WasteManagementSystem.Models;
using WasteManagementSystem.Models.DTOs;


namespace WasteManagementSystem.Controllers
{
    public class ItemsController : Controller
    {
        private readonly WasteContext _context;

        public ItemsController(WasteContext context)
        {
            _context = context;
        }

        // GET: Items
        public async Task<IActionResult> Index()
        {
            var items = await _context.Items
                .Include(i => i.House)
                .ToListAsync();

            var dtoData = items.Select(i => new ItemDTO
            {
                Id = i.Id,
                Name = i.ItemName,
                Category = i.Category.ToString(),
                FinancialValue = i.Value,
                DaysRemaining = (i.ExpiryDate - DateTime.Now).Days > 0
                        ? (i.ExpiryDate - DateTime.Now).Days.ToString()
                        : "0",
                ExpiryStatus = (i.ExpiryDate < DateTime.Now) ? "Expired" : "Safe",
                HouseAddress = i.House?.Address ?? "No House Assigned"
            }).ToList();

            return View(dtoData);
        }

        // GET: Items/Details/5
        public async Task<IActionResult> Details(int? id)  
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.Items
                .Include(i => i.House)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        // GET: Items/Create
        public IActionResult Create()
        {
            ViewData["HouseDetailsId"] = new SelectList(_context.Houses, "Id", "Address");
            return View();
        }

        // POST: Items/Create
        // This reverse maps the content and turns it into a database item from a dto object. If fails it returns the user to the view.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ItemName,Category,PurchaseDate,ExpiryDate,Value,HouseDetailsId")] Item item)
        {
            if (ModelState.IsValid)
            {
                _context.Add(item);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["HouseDetailsId"] = new SelectList(_context.Houses, "Id", "Address", item.HouseDetailsId);
            return View(item);
        }

        // GET: Items/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.Items.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            ViewData["HouseDetailsId"] = new SelectList(_context.Houses, "Id", "Address", item.HouseDetailsId);
            return View(item);
        }

        // POST: Items/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ItemName,Category,PurchaseDate,ExpiryDate,Status,Value,HouseDetailsId")] Item item)
        {
            if (id != item.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(item);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ItemExists(item.Id))
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
            ViewData["HouseDetailsId"] = new SelectList(_context.Houses, "Id", "Address", item.HouseDetailsId);
            return View(item);
        }

        // GET: Items/Delete/5
        // Fetches an item and maps the item into a dto object to be displayed on the delete confirmation page. 
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var item = await _context.Items
                .Include(i => i.House)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (item == null) return NotFound();

            var dto = new ItemDTO
            {
                Id = item.Id,
                Name = item.ItemName,
                Category = item.Category.ToString(),
                HouseAddress = item.House?.Address ?? "Unassigned",
                WasteImpact = item.Value > 10 ? "High Priority" : "Low"
            };

            return View(dto);
        }

        // POST: Items/Delete/5
        // Actually deletes the record from the database. If the item is not found it returns the user to the index page.
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var item = await _context.Items.FindAsync(id);
            if (item != null)
            {
                _context.Items.Remove(item);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ItemExists(int id)
        {
            return _context.Items.Any(e => e.Id == id);
        }
    }
}
