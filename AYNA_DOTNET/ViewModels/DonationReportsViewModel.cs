using Ayna.Models;

namespace Ayna.ViewModels.FarmerVMs
{
    public class DonationReportsViewModel
    {
        public Models.Farmer Farmer { get; set; }
        public List<Donation> Donations { get; set; } = new();
        public int TotalDonations { get; set; }
        public int PendingDonations { get; set; }
        public int ApprovedDonations { get; set; }
        public int DeliveredDonations { get; set; }

        /// <summary>
        /// Gets donation statistics grouped by month
        /// </summary>
        public Dictionary<int, int> MonthlyDonations =>
            Donations
                .GroupBy(d => d.DonDate.Month)
                .ToDictionary(g => g.Key, g => g.Count());

        /// <summary>
        /// Gets donation statistics grouped by status
        /// </summary>
        public Dictionary<string, int> StatusStatistics =>
            Donations
                .GroupBy(d => d.DonStatus)
                .ToDictionary(g => g.Key, g => g.Count());
    }
}
