namespace Ayna.ViewModels.DonorVMs
{
    public class DonationItemDto
    {
        public string BasketName { get; set; }
        public int? Quantity { get; set; }
        public decimal? Price { get; set; }
        public List<CropInfoDto> Crops { get; set; } = new();
    }
}
