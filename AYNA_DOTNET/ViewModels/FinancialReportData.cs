namespace Ayna.ViewModels.AdminVMs
{
    public class FinancialReportData
    {
        public decimal TotalRevenue { get; set; }
        public decimal TotalPayments { get; set; }
        public Dictionary<string, int> PaymentMethods { get; set; }
        public Dictionary<DateOnly, decimal> RevenueByDay { get; set; }
    }
}
