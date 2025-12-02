namespace Ayna.ViewModels.AdminVMs
{
    public class OrdersReportData
    {
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public Dictionary<string, int> OrdersByStatus { get; set; }
        public Dictionary<DateOnly, decimal> RevenuePerDay { get; set; }
    }
}
