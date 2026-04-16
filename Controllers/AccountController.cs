using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WasteManagementSystem.Data;
using WasteManagementSystem.Models;
using WasteManagementSystem.Models.DTOs;

namespace WasteManagementSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly WasteContext _context;
        private readonly IPasswordHasher<HouseDetails> _passwordHasher;

        public AccountController(WasteContext context, IPasswordHasher<HouseDetails> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        // GET: Account/Register
        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(HouseDetails model)
        {
            if (ModelState.IsValid)
            {
                if (await _context.Houses.AnyAsync(h => h.Eircode == model.Eircode))
                {
                    ModelState.AddModelError("Eircode", "This household is already registered.");
                    return View(model);
                }

                model.Role = "User";

                model.Password = _passwordHasher.HashPassword(model, model.Password);

                _context.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction("Login");
            }
            return View(model);
        }

        // GET: Account/Login
        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string eircode, string password)
        {
            var house = await _context.Houses.FirstOrDefaultAsync(h => h.Eircode == eircode);

            if (house != null)
            {
                var result = _passwordHasher.VerifyHashedPassword(house, house.Password, password);

                if (result == PasswordVerificationResult.Success)
                {
                    var claims = new List<Claim> {
                        new Claim(ClaimTypes.Name, house.Eircode),
                        new Claim("Address", house.Address),
                        new Claim(ClaimTypes.Role, house.Role ?? "User")
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity));

                    if (house.Role == "Admin")
                    {
                        return RedirectToAction("Index", "Admin");
                    }

                    return RedirectToAction("Index", "Items");
                }
            }

            ViewBag.Error = "Invalid Eircode or Password";
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}
