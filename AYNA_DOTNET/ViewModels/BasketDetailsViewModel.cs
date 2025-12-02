namespace Ayna.ViewModels.FarmerVMs
{
    public class BasketDetailsViewModel
    {
        public int BasId { get; set; }
        public string BasContent { get; set; }
        public decimal BasPrice { get; set; }
        public int BasQty { get; set; }
        public DateTime AddedAt { get; set; }
        public List<BasketCropDetail> Crops { get; set; } = new();
    }
}
