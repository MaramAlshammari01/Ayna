namespace Ayna.ViewModels.AdminVMs
{
    public class RecentUserDto
    {
        public int UserId { get; set; }
        public string UserEmail { get; set; }
        public string UserType { get; set; }
        public string UserStatus { get; set; }
        public DateTime? AddedAt { get; set; }
        public string Name { get; set; }
    }
}
