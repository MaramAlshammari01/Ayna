using Ayna.Models;

namespace Ayna.ViewModels.FarmerVMs
{
    public class PerformanceReportViewModel
    {
        public Models.Farmer Farmer { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        // Data
        public List<Donation> Donations { get; set; } = new();
        public List<Basket> Baskets { get; set; } = new();

        // Statistics
        public DonationPerformanceStats DonationStats { get; set; }
        public SalesPerformanceStats SalesStats { get; set; }

        // Trends
        public List<MonthlyDonationTrend> MonthlyDonations { get; set; } = new();
        public List<MonthlySalesTrend> MonthlySales { get; set; } = new();
    }
}
