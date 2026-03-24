using Microsoft.EntityFrameworkCore;
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
                if (context.Items.Any()) return;

                var testHouse = new HouseDetails
                {
                    Address = "123 Green Lane, Drogheda",
                    Eircode = "A65 F4E2",
                    Email = "test@dkit.ie",
                    Password = "Password123!" // hash this yoke
                };

                context.Houses.Add(testHouse);
                context.SaveChanges();

                context.Items.AddRange(
                    new Item
                    {
                        ItemName = "Milk",
                        Category = ItemCategory.Dairy,
                        PurchaseDate = DateTime.Now.AddDays(-2),
                        ExpiryDate = DateTime.Now.AddDays(3),
                        Value = 1.50m,
                        HouseDetailsId = testHouse.Id
                    },
                    new Item
                    {
                        ItemName = "Spinach",
                        Category = ItemCategory.Vegetable,
                        PurchaseDate = DateTime.Now.AddDays(-1),
                        ExpiryDate = DateTime.Now.AddDays(-1), //expired test data
                        Value = 2.00m,
                        HouseDetailsId = testHouse.Id
                    }
                );
                context.SaveChanges();
            }
        }
    }
}
