using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WasteManagementSystem.Data;
using WasteManagementSystem.Models;
using WasteManagementSystem.Models.DTOs;


namespace WasteManagementSystem.Controllers
{
    [Authorize]
    public class ItemsController : Controller
    {
        private readonly WasteContext _context;

        public ItemsController(WasteContext context)
        {
            _context = context;
        }

        // GET: Items
        public async Task<IActionResult> Index(string searchString, string itemCategory)
        {
            string userEircode = User.Identity.Name;

            var itemsQuery = _context.Items
                .Include(i => i.House)
                .Where(i => i.HouseDetailsId == userEircode && i.Status == ItemStatus.InPantry)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                itemsQuery = itemsQuery.Where(s => s.ItemName.Contains(searchString));
            }

            if (!string.IsNullOrEmpty(itemCategory) && Enum.TryParse(itemCategory, out ItemCategory categoryEnum))
            {
                itemsQuery = itemsQuery.Where(x => x.Category == categoryEnum);
            }

            var items = await itemsQuery.ToListAsync();

            var dtoData = items.Select(i => new ItemDTO
            {
                Id = i.Id,
                Name = i.ItemName,
                Category = i.Category.ToString(),
                HouseAddress = i.House?.Address ?? "No House",
                DaysRemaining = (i.ExpiryDate - DateTime.Now).Days.ToString(),
                ExpiryStatus = (i.ExpiryDate < DateTime.Now) ? "Expired" : "Safe",
                WasteImpact = i.Value > 10 ? "High" : "Low"
            }).ToList();

            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentCategory"] = itemCategory;

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

        
        // GET: Items/WasteHistory
        public async Task<IActionResult> WasteHistory()
        {
            string userEircode = User.Identity.Name;

            var history = await _context.WasteLogs
                .Include(w => w.Item)
                .Where(w => w.Item.HouseDetailsId == userEircode)
                .OrderByDescending(w => w.DateWasted)
                .ToListAsync();

            return View(history);
        }

        // GET: Items/LogWaste/5
        public async Task<IActionResult> LogWaste(int? id)
        {
            if (id == null) return NotFound();

            var item = await _context.Items.FindAsync(id);
            if (item == null) return NotFound();

            ViewBag.ItemName = item.ItemName;

            var wasteLog = new WasteLog
            {
                ItemId = item.Id,
                DateWasted = DateTime.Now
            };

            return View(wasteLog);
        }

        // POST: Items/LogWaste/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogWaste(WasteLog wasteLog)
        {
            if (ModelState.IsValid)
            {
                _context.WasteLogs.Add(wasteLog);

                var item = await _context.Items.FindAsync(wasteLog.ItemId);
                if (item != null)
                {
                    item.Status = ItemStatus.Wasted;
                    _context.Update(item);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(wasteLog);
        }

        // GET: Items/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Items/Create
        // This reverse maps the content and turns it into a database item from a dto object. If fails it returns the user to the view.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ItemName,Category,PurchaseDate,ExpiryDate,Value,HouseDetailsId")] Item item)
        {
            item.HouseDetailsId = User.Identity.Name;

            item.Status = ItemStatus.InPantry;

            ModelState.Remove("HouseDetailsId");
            ModelState.Remove("House");

            if (ModelState.IsValid)
            {
                _context.Add(item);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(item);
        }

        // GET: Items/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var item = await _context.Items.FindAsync(id);
            if (item == null) return NotFound();

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
                    }else throw;
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

        [Authorize]
        public async Task<IActionResult> Analytics()
        {
            var eircode = User.Identity.Name;
            var house = await _context.Houses.FindAsync(eircode);

            //feature for premium users
            if (house == null || (!house.IsPremium && !User.IsInRole("Admin")))
            {
                return RedirectToAction("Upgrade", "Account");
            }

            var logs = await _context.WasteLogs
                .Include(l => l.Item)
                .Where(l => l.Item.HouseDetailsId == eircode)
                .ToListAsync();

            var categoryData = logs.GroupBy(l => l.Item.Category)
                .Select(g => new {
                    Category = g.Key.ToString(),
                    Count = g.Count(),
                    TotalValue = g.Sum(x => x.Item.Value) //financial impact
                }).ToList();

            ViewBag.CategoryData = categoryData;

            ViewBag.TotalItemsWasted = logs.Count;
            ViewBag.TotalFinancialLoss = logs.Sum(l => l.Item.Value);

            return View(logs);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult DevDashboard()
        {
            return View();
        }

        private bool ItemExists(int id) => _context.Items.Any(e => e.Id == id);
    }
}
