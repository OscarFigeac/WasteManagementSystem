using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using WasteManagementSystem.Models;

namespace WasteManagementSystem.Data
{
    public class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new WasteContext(
                serviceProvider.GetRequiredService<DbContextOptions<WasteContext>>()))
            {
                context.Database.EnsureCreated();

                if (context.Houses.Any()) return;

                var hasher = serviceProvider.GetRequiredService<IPasswordHasher<HouseDetails>>();

                var testHouse = new HouseDetails
                {
                    Address = "123 Green Lane, Drogheda",
                    Eircode = "A65F4E2",
                    Password = "" //temporary
                };

                testHouse.Password = hasher.HashPassword(testHouse, "Password123!");

                context.Houses.Add(testHouse);
                context.SaveChanges();

                if (!context.Houses.Any(h => h.Role == "Admin"))
                {
                    var adminHouse = new HouseDetails
                    {
                        Eircode = "D02X285", // Use a real or test Eircode
                        Address = "System Administrator Office",
                        Role = "Admin"
                    };

                    // password: Admin123!
                    adminHouse.Password = hasher.HashPassword(adminHouse, "Admin123!");

                    context.Houses.Add(adminHouse);
                    context.SaveChanges();

                    context.Items.AddRange(
                    new Item
                    {
                        ItemName = "Milk",
                        Category = ItemCategory.Dairy,
                        PurchaseDate = DateTime.Now.AddDays(-2),
                        ExpiryDate = DateTime.Now.AddDays(3),
                        Value = 1.50m,
                        HouseDetailsId = testHouse.Eircode
                    },
                    new Item
                    {
                        ItemName = "Spinach",
                        Category = ItemCategory.Vegetable,
                        PurchaseDate = DateTime.Now.AddDays(-1),
                        ExpiryDate = DateTime.Now.AddDays(-1),
                        Value = 2.00m,
                        HouseDetailsId = testHouse.Eircode
                    }
                );
                    context.SaveChanges();
                }
            }
        }
    }
}