namespace WasteManagementSystem.Services
{
    public static class MapLocationServices
    {
        public static readonly Dictionary<string, (double Lat, double Lng)> AreaCoordinates = new()
        {
            { "A91", (53.985, -6.350) }, // Dundalk
            { "A92", (53.714, -6.349) }, // Drogheda
            { "A96", (53.864, -6.442) }, // Ardee
            { "C15", (53.652, -6.677) }, // Navan
            { "C63", (53.535, -6.657) },  // Trim
            { "C12", (53.606, -6.877) }, // Kells
            { "A85", (53.509, -6.449) }, // Ashbourne
            { "A83", (53.428, -6.438) },  // Dunboyne
            { "V94", (52.660, -8.620) },  // Limerick
            { "D01", (53.350, -6.240) }, // Dublin 1
            { "D02", (53.340, -6.250) }, // Dublin 2
        };
    }
}
