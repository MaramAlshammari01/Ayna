namespace Ayna.ViewModels.AdminVMs
{
    public class DonationStatsDto
    {
        public int Total { get; set; }
        public int Pending { get; set; }
        public int Approved { get; set; }
        public int Delivered { get; set; }
        public int Rejected { get; set; }
    }
}
