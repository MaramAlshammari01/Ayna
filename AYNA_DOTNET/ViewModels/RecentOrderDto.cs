namespace Ayna.ViewModels.DonorVMs
{
    public class RecentOrderDto
    {
        public int OrdId { get; set; }
        public string CharityName { get; set; }
        public decimal? OrdPrice { get; set; }
        public string OrdStatus { get; set; }
        public DateOnly? OrdDate { get; set; }
        public int ItemsCount { get; set; }
    }
}
