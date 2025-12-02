namespace Ayna.ViewModels.CharityVMs
{
    public class AvailableDonationDto
    {
        public int DonId { get; set; }
        public string FarmerName { get; set; }
        public string FarmerLocation { get; set; }
        public string DonDescription { get; set; }
        public DateTime? DonDate { get; set; }
        public List<DonationCropDto> Crops { get; set; } = new();
    }
}
