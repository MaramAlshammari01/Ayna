using Ayna.Models;

namespace Ayna.ViewModels.FarmerVMs
{
    public class DonationIndexViewModel
    {
        public List<Donation> Donations { get; set; } = new();
        public Models.Farmer Farmer { get; set; }
        public List<Charity> Charities { get; set; } = new();
        public List<Crop> AvailableCrops { get; set; } = new();
    }
}
