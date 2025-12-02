namespace Ayna.ViewModels.CharityVMs
{
    public class OrderDto
    {
        public int OrdId { get; set; }
        public string DonorName { get; set; }
        public decimal? OrdPrice { get; set; }
        public string OrdStatus { get; set; }
        public DateOnly? OrdDate { get; set; }
        public TimeOnly? OrdTime { get; set; }
        public int ItemsCount { get; set; }
    }
}
