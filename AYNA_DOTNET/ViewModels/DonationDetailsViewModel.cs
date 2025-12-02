namespace Ayna.ViewModels.FarmerVMs
{
    public class DonationDetailsViewModel
    {
        public int DonId { get; set; }
        public string DonDescription { get; set; }
        public string DonStatus { get; set; }
        public DateTime? DonDate { get; set; }
        public List<DonationCropDetail> Crops { get; set; } = new();
        public List<DonationRequestDetail> Requests { get; set; } = new();
    }
}
