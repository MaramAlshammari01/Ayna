namespace Ayna.ViewModels.CharityVMs
{
    public class MyRequestsViewModel
    {
        public List<DonationRequestDto> Requests { get; set; } = new();
        public string FilterStatus { get; set; }
    }
}
