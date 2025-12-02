namespace Ayna.ViewModels.FarmerVMs
{
    public class AvailableCropViewModel
    {
        public int CroId { get; set; }
        public string CroName { get; set; }
        public string CroType { get; set; }
        public int CroQuantity { get; set; }
        public string CroUnit { get; set; }
        public DateTime ExpiredAt { get; set; }
        public string DisplayName => $"{CroName} ({CroQuantity} {CroUnit})";
    }
}
