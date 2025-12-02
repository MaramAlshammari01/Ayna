namespace Ayna.ViewModels.CharityVMs
{
    public class CharityDashboardViewModel
    {
        public CharityInfoDto CharityInfo { get; set; }

        // Statistics
        public int TotalOrders { get; set; }
        public int TotalDonationRequests { get; set; }
        public int PendingRequests { get; set; }
        public int CompletedOrders { get; set; }

        // Recent Activities
        public List<RecentOrderDto> RecentOrders { get; set; } = new();
        public List<RecentDonationRequestDto> RecentDonationRequests { get; set; } = new();
    }
}
