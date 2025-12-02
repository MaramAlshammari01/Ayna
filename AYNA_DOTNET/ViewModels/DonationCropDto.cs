namespace Ayna.ViewModels.CharityVMs
{
    public class DonationCropDto
    {
        public string CropName { get; set; }
        public string CropType { get; set; }
        public int? Quantity { get; set; }
        public string Unit { get; set; }
        public decimal? Weight { get; set; }
        public string ShelfLife { get; set; }
    }
}
