namespace Ayna.ViewModels.CharityVMs
{
    public class AvailableDonationsViewModel
    {
        public List<AvailableDonationDto> Donations { get; set; } = new();
        public string SearchQuery { get; set; }
        public string CropTypeFilter { get; set; }
    }

}
