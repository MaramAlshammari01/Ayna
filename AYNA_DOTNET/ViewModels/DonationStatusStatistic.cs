namespace Ayna.ViewModels.FarmerVMs
{
    public class DonationStatusStatistic
    {
        public string Status { get; set; }
        public int Count { get; set; }
        public string DisplayStatus => GetArabicStatus(Status);

        private string GetArabicStatus(string status)
        {
            return status switch
            {
                "Pending" => "قيد الانتظار",
                "Approved" => "موافق عليه",
                "PickedUp" => "تم الاستلام",
                "Delivered" => "تم التسليم",
                "Rejected" => "مرفوض",
                _ => status
            };
        }
    }
}
