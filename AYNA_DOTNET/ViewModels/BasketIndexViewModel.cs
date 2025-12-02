using Ayna.Models;

namespace Ayna.ViewModels.FarmerVMs
{
    public class BasketIndexViewModel
    {
        public List<Basket> Baskets { get; set; } = new();
        public Models.Farmer Farmer { get; set; }
        public List<Crop> AvailableCrops { get; set; } = new();
    }
}
