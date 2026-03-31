namespace WasteManagementSystem.Models.DTOs
{
    public class ItemDTO
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string ExpiryStatus { get; set; } = string.Empty;
        public string DaysRemaining { get; set; } = string.Empty;
        public decimal FinancialValue { get; set; }

        public string HouseAddress { get; set; } = string.Empty;

        //Reference to the WasteLog class
        public bool IsWasted { get; set; }

        //This field is exclusively for the purpose of displaying the impact this has on the environment. Update on Controller when necessary.
        public string WasteImpact { get; set; } = string.Empty;
    }
}
