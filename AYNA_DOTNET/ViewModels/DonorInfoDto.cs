namespace Ayna.ViewModels.DonorVMs
{
    public class DonorInfoDto
    {
        public int DonorId { get; set; }
        public string DonorFirstName { get; set; }
        public string DonorLastName { get; set; }
        public string FullName => $"{DonorFirstName} {DonorLastName}";
    }
}
