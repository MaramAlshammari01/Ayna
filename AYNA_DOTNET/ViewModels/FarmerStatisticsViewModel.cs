namespace Ayna.ViewModels.FarmerVMs
{
    public class FarmerStatisticsViewModel
    {
        public List<CropTypeStatistic> CropStatistics { get; set; } = new();
        public List<MonthlySalesStatistic> SalesStatistics { get; set; } = new();
        public List<DonationStatusStatistic> DonationStatistics { get; set; } = new();
        public decimal TotalRevenue { get; set; }
        public int TotalDonations { get; set; }
        public int TotalOrders { get; set; }
    }
}
