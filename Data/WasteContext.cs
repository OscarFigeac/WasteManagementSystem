using Microsoft.EntityFrameworkCore;
using WasteManagementSystem.Models;

namespace WasteManagementSystem.Data
{
    public class WasteContext : DbContext
    {
        public WasteContext(DbContextOptions<WasteContext> options)
            : base(options)
        {
        }

        // These define your database tables
        public DbSet<HouseDetails> Houses { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<WasteLog> WasteLogs { get; set; }
    }
}
}
