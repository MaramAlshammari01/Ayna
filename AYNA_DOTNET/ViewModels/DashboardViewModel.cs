namespace Ayna.ViewModels.AdminVMs
{
    public class DashboardViewModel
    {
        // User Statistics
        public int TotalUsers { get; set; }
        public int TotalFarmers { get; set; }
        public int TotalCharities { get; set; }
        public int TotalDonors { get; set; }

        // Donation Statistics
        public int TotalDonations { get; set; }
        public int TotalOrders { get; set; }
        public int PendingDonations { get; set; }
        public int CompletedOrders { get; set; }

        // Recent Activities
        public List<RecentUserDto> RecentUsers { get; set; } = new();
        public List<RecentDonationDto> RecentDonations { get; set; } = new();
    }
}
