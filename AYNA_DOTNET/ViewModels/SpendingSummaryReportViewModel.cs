namespace Ayna.ViewModels.DonorVMs
{
    public class SpendingSummaryReportViewModel
    {
        public List<MonthlySummaryDto> MonthlySummary { get; set; } = new();
        public List<CharitySummaryDto> CharitySummary { get; set; } = new();
        public decimal TotalSpent { get; set; }
        public int TotalOrders { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

}
