namespace Ayna.ViewModels.AdminVMs
{
    public class DonationsIndexViewModel
    {
        public List<DonationListItemViewModel> Donations { get; set; } = new();
        public string FilterStatus { get; set; }
        public DateTime? FilterStartDate { get; set; }
        public DateTime? FilterEndDate { get; set; }
    }
}
