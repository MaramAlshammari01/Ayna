namespace Ayna.ViewModels.FarmerVMs
{
    public class DonationPerformanceStats
    {
        public int TotalDonations { get; set; }
        public int PendingDonations { get; set; }
        public int ApprovedDonations { get; set; }
        public int DeliveredDonations { get; set; }
        public int TotalDonatedQuantity { get; set; }
        public int DonationImpact { get; set; }

        /// <summary>
        /// Gets the delivery rate as a percentage
        /// </summary>
        public double DeliveryRate =>
            TotalDonations > 0 ? (double)DeliveredDonations / TotalDonations * 100 : 0;

        /// <summary>
        /// Gets the approval rate as a percentage
        /// </summary>
        public double ApprovalRate =>
            TotalDonations > 0 ? (double)ApprovedDonations / TotalDonations * 100 : 0;
    }
}
