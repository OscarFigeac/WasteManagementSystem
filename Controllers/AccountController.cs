using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Stripe.Checkout;
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

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ProcessPayment()
        {
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
        {
            new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmount = 499, //€4.99 
                    Currency = "eur",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = "MyWaste Premium Membership",
                    },
                },
                Quantity = 1,
            },
        },
                Mode = "payment",
                SuccessUrl = "https://localhost:7112/Account/PaymentSuccess",
                CancelUrl = "https://localhost:7112/Account/Upgrade",
            };

            var service = new SessionService();
            Session session = await service.CreateAsync(options);

            return Redirect(session.Url);
        }

        [Authorize]
        public async Task<IActionResult> PaymentSuccess()
        {
            var eircode = User.Identity.Name;

            var house = await _context.Houses.FindAsync(eircode);

            if (house != null)
            {
                house.IsPremium = true;
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Thank you! Your account has been upgraded to Premium.";
            }

            return View();
        }

        [Authorize]
        public IActionResult Upgrade()
        {
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}
