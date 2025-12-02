namespace Ayna.ViewModels.DonorVMs
{
    public class BasketDto
    {
        public int BasId { get; set; }
        public string BasContent { get; set; }
        public decimal? BasPrice { get; set; }
        public int? BasQty { get; set; }
        public string FarmerName { get; set; }
        public string FarmerLocation { get; set; }
        public List<BasketCropDto> Crops { get; set; } = new();
    }
}
