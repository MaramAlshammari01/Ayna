
namespace Ayna.ViewModels.DonorVMs
{
    public class DonorDashboardViewModel
    {
        public DonorInfoDto DonorInfo { get; set; }

        // Statistics
        public int TotalDonations { get; set; }
        public decimal TotalAmount { get; set; }
        public int ActivePayments { get; set; }
        public int SupportedCharities { get; set; }

        // Recent Activities
        public List<RecentOrderDto> RecentDonations { get; set; } = new();
        public List<FeaturedBasketDto> FeaturedBaskets { get; set; } = new();
        public int CartCount { get; set; }
    }

}
