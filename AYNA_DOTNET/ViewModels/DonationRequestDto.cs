namespace Ayna.ViewModels.CharityVMs
{
    public class DonationRequestDto
    {
        public int ReqId { get; set; }
        public int DonId { get; set; }
        public string FarmerName { get; set; }
        public string ReqDonation { get; set; }
        public string ReqStatus { get; set; }
        public string PickupDuration { get; set; }
        public DateTime? AddedAt { get; set; }
        public int CropsCount { get; set; }
    }
}
