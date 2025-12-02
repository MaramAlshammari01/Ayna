namespace Ayna.ViewModels.DonorVMs
{
    public class DonationsViewModel
    {
        public List<DonationDto> Donations { get; set; } = new();
        public DonorInfoDto DonorInfo { get; set; }
    }
}
