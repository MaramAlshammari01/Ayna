using Ayna.Models;

namespace Ayna.ViewModels.FarmerVMs
{
    public class CropIndexViewModel
    {
        public List<Crop> Crops { get; set; } = new();
        public Models.Farmer Farmer { get; set; }
    }
}
