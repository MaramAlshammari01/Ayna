using Ayna.Models;

namespace Ayna.ViewModels.FarmerVMs
{
    public class FarmerDashboardViewModel
    {
        public Models.Farmer Farmer { get; set; }
        public int TotalCrops { get; set; }
        public int ActiveBaskets { get; set; }
        public int PendingDonations { get; set; }
        public int CompletedOrders { get; set; }
        public List<Crop> RecentCrops { get; set; } = new();
        public List<Donation> RecentDonations { get; set; } = new();

        /// <summary>
        /// Gets the farmer's full name
        /// </summary>
        public string FullName => $"{Farmer?.FarFirstName} {Farmer?.FarLastName}";
    }
}
