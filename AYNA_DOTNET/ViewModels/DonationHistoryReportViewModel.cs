namespace Ayna.ViewModels.DonorVMs
{
    public class DonationHistoryReportViewModel
    {
        public List<DonationHistoryDto> Donations { get; set; } = new();
        public decimal TotalAmount { get; set; }
        public int DonationsCount { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string StatusFilter { get; set; }
    }
}
