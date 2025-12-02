namespace Ayna.ViewModels.CharityVMs
{
    public class OrderDetailsViewModel
    {
        public int OrdId { get; set; }
        public string DonorName { get; set; }
        public decimal? OrdPrice { get; set; }
        public string OrdStatus { get; set; }
        public DateOnly? OrdDate { get; set; }
        public TimeOnly? OrdTime { get; set; }
        public DateTime? AddedAt { get; set; }
        public List<OrderItemDto> Items { get; set; } = new();
        public PaymentInfoDto PaymentInfo { get; set; }
    }
}
