namespace Ayna.ViewModels.FarmerVMs
{
    public class CropDetailsViewModel
    {
        public int CroId { get; set; }
        public string CroName { get; set; }
        public string CroType { get; set; }
        public decimal CroWeight { get; set; }
        public int CroQuantity { get; set; }
        public string CroUnit { get; set; }
        public string CroShelfLife { get; set; }
        public DateTime ExpiredAt { get; set; }
        public DateTime AddedAt { get; set; }
        public int DaysUntilExpiry { get; set; }
        public string ExpiryStatus { get; set; }
    }
}
