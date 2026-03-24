using System.ComponentModel.DataAnnotations;

namespace WasteManagementSystem.Models
{
    public class WasteLog
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ItemId { get; set; }

        [Required(ErrorMessage = "Please provide a reason for the waste")]
        public string Reason { get; set; } = string.Empty;

        [Required, DataType(DataType.Date)]
        public DateTime DateWasted { get; set; }

        public virtual Item? Item { get; set; }
    }
}
