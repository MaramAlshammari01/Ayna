namespace Ayna.ViewModels.AdminVMs
{
    public class UserDetailsViewModel
    {
        public int UserId { get; set; }
        public string UserEmail { get; set; }
        public string UserPhone { get; set; }
        public string UserType { get; set; }
        public string UserStatus { get; set; }
        public DateTime? AddedAt { get; set; }

        // Type-specific details
        public FarmerDetailsDto FarmerDetails { get; set; }
        public CharityDetailsDto CharityDetails { get; set; }
        public DonorDetailsDto DonorDetails { get; set; }
    }
}
