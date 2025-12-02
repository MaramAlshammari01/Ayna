namespace Ayna.ViewModels.FarmerVMs
{
    public class CropInventoryItem
    {
        public int CropId { get; set; }
        public string CropName { get; set; }
        public string CropType { get; set; }
        public int Quantity { get; set; }
        public string Unit { get; set; }
        public decimal Weight { get; set; }
        public DateTime ExpiredAt { get; set; }
        public DateTime AddedAt { get; set; }
        public int DaysUntilExpiry { get; set; }
        public string ExpiryStatus { get; set; }
        public int UsedInBaskets { get; set; }
        public int RemainingQuantity { get; set; }

        /// <summary>
        /// Gets the expiry status display text
        /// </summary>
        public string ExpiryStatusDisplay =>
            ExpiryStatus switch
            {
                "expired" => "منتهي الصلاحية",
                "expiring_soon" => "ينتهي قريباً",
                "expiring_later" => "ينتهي لاحقاً",
                "good" => "جيد",
                _ => ExpiryStatus
            };

        /// <summary>
        /// Gets the Bootstrap color class for the expiry status
        /// </summary>
        public string ExpiryStatusColor =>
            ExpiryStatus switch
            {
                "expired" => "danger",
                "expiring_soon" => "warning",
                "expiring_later" => "info",
                "good" => "success",
                _ => "secondary"
            };
    }
}
