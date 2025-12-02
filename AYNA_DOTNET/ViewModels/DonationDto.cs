namespace Ayna.ViewModels.DonorVMs
{
    public class DonationDto
    {
        public int OrdId { get; set; }
        public string CharityName { get; set; }
        public decimal? OrdPrice { get; set; }
        public string OrdStatus { get; set; }
        public DateOnly? OrdDate { get; set; }
        public TimeOnly? OrdTime { get; set; }
        public List<DonationItemDto> Items { get; set; } = new();
        public PaymentInfoDto PaymentInfo { get; set; }
    }
}
