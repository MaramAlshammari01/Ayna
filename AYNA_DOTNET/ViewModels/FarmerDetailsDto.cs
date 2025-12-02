namespace Ayna.ViewModels.AdminVMs
{
    public class FarmerDetailsDto
    {
        public int FarId { get; set; }
        public string FarFirstName { get; set; }
        public string FarLastName { get; set; }
        public string FarLocation { get; set; }
        public int CropsCount { get; set; }
        public int DonationsCount { get; set; }
    }
}
