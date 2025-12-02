namespace Ayna.ViewModels.CharityVMs
{
    public class RecentDonationRequestDto
    {
        public int ReqId { get; set; }
        public int DonId { get; set; }
        public string FarmerName { get; set; }
        public string ReqDonation { get; set; }
        public string ReqStatus { get; set; }
        public DateTime? AddedAt { get; set; }
    }
}
