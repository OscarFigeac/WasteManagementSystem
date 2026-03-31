namespace WasteManagementSystem.Models.DTOs
{
    public class ItemDTO
    {
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string ExpiryStatus { get; set; } = string.Empty;
        public string DaysRemaining { get; set; } = string.Empty;
        public decimal FinancialValue { get; set; }
    }
}
