namespace Ayna.ViewModels.FarmerVMs
{
    public class CropInventoryReportViewModel
    {
        public Models.Farmer Farmer { get; set; }
        public List<CropInventoryItem> Crops { get; set; } = new();
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        // Statistics
        public int TotalCrops { get; set; }
        public int ExpiringSoon { get; set; }
        public int Expired { get; set; }
        public int TotalQuantity { get; set; }
        public decimal TotalValue { get; set; }
    }
}
