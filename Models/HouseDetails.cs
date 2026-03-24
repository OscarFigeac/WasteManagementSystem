using System.ComponentModel.DataAnnotations;

namespace WasteManagementSystem.Models
{
    public class HouseDetails
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Address { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^[A-Z][0-9][0-9W] [A-Z0-9]{4}$", ErrorMessage = "Invalid Eircode format (e.g. D00 D0D0)")] //Following the eircode syntax before inserting into the database
        public string Eircode { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, DataType(DataType.Password)]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
        public string Password { get; set; } = string.Empty;

        // Navigation Property: One House has many Items
        public virtual ICollection<Item> Items { get; set; } = new List<Item>();
    }
}
