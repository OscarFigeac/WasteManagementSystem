namespace WasteManagementSystem.Models.DTOs
{
    public class HouseDTO
    {
        public int Id { get; set; }
        public string Address { get; set; } = string.Empty;
        public int ItemCount { get; set; }
    }
}
