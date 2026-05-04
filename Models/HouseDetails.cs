using System.ComponentModel.DataAnnotations;

namespace WasteManagementSystem.Models
{
    public class HouseDetails
    {

        [Key]
        [Required]
        [RegularExpression(@"^[A-Z][0-9][0-9W] [A-Z0-9]{4}$", ErrorMessage = "Invalid Eircode format (e.g. D00 D0D0)")] //Following the eircode syntax before inserting into the database
        public string Eircode { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters.")]
        public string Password { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Address { get; set; } = string.Empty;

        public virtual ICollection<Item> Items { get; set; } = new List<Item>();

        //for admin purposes, to differentiate between users and admin accounts
        public string Role { get; set; } = "User";

        //false for base accounts, true for premium.
        [Display(Name = "Account Type")]
        public bool IsPremium { get; set; } = false;
    }
}
