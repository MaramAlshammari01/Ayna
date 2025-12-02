namespace Ayna.ViewModels.AdminVMs
{
    public class DonationsReportData
    {
        public int TotalDonations { get; set; }
        public Dictionary<string, int> DonationsByStatus { get; set; }
        public Dictionary<DateTime, int> DonationsPerDay { get; set; }
    }
}
