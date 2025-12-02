namespace Ayna.ViewModels.FarmerVMs
{
    public class SalesPerformanceStats
    {
        public int TotalBaskets { get; set; }
        public int ActiveBaskets { get; set; }
        public int SoldBaskets { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AverageBasketPrice { get; set; }

        /// <summary>
        /// Gets the sales rate as a percentage
        /// </summary>
        public double SalesRate =>
            TotalBaskets > 0 ? (double)SoldBaskets / TotalBaskets * 100 : 0;
    }
}
