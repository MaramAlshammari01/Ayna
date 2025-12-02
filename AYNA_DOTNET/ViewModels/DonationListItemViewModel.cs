namespace Ayna.ViewModels.AdminVMs
{
    public class DonationListItemViewModel
    {
        public int DonId { get; set; }
        public string FarmerName { get; set; }
        public string DonDescription { get; set; }
        public string DonStatus { get; set; }
        public DateTime? DonDate { get; set; }
        public DateTime? AddedAt { get; set; }
        public int CropsCount { get; set; }
        public int RequestsCount { get; set; }
    }
}
