namespace Ayna.ViewModels.DonorVMs
{
    public class MonthlySummaryDto
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public string MonthName { get; set; }
        public int Count { get; set; }
        public decimal Total { get; set; }
    }
}
