namespace Ayna.ViewModels.AdminVMs
{
    public class RecentDonationDto
    {
        public int DonId { get; set; }
        public string FarmerName { get; set; }
        public string DonDescription { get; set; }
        public string DonStatus { get; set; }
        public DateTime? DonDate { get; set; }
        public int CropCount { get; set; }
    }
}
