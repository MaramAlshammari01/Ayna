namespace Ayna.ViewModels.DonorVMs
{
    public class PaymentsViewModel
    {
        public List<PaymentDto> Payments { get; set; } = new();
        public DonorInfoDto DonorInfo { get; set; }
    }

}
