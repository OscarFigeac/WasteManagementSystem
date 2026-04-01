using System.ComponentModel.DataAnnotations;

namespace WasteManagementSystem.Models
{
    public class Item
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Item name is required")]
        public string ItemName { get; set; } = string.Empty;

        [Required]
        public ItemCategory Category { get; set; }

        [Required, DataType(DataType.Date)]
        [Display(Name = "Date Purchased")]
        public DateTime PurchaseDate { get; set; }

        [Required, DataType(DataType.Date)]
        [Display(Name = "Expiry Date")]
        public DateTime ExpiryDate { get; set; }

        public ItemStatus Status { get; set; } = ItemStatus.InPantry;

        [Required, Range(0.01, 1000.00)] //Monthly estimated value? Could increment if needed for display purposes
        [Display(Name = "Estimated Value (€)")]
        public decimal Value { get; set; }

        [Required]
        public string HouseDetailsId { get; set; } = string.Empty;
        public virtual HouseDetails? House { get; set; }

        public virtual WasteLog? WasteLog { get; set; }
    }
}
