namespace Ayna.ViewModels.DonorVMs
{
    public class PaymentDto
    {
        public int PayId { get; set; }
        public string CharityName { get; set; }
        public string PayMethod { get; set; }
        public decimal? PayAmount { get; set; }
        public string PayStatus { get; set; }
        public DateOnly? PayDate { get; set; }
        public TimeOnly? PayTime { get; set; }
        public string TransactionId { get; set; }
        public int? OrdId { get; set; }
    }
}
