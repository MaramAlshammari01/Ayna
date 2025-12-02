namespace Ayna.ViewModels.AdminVMs
{
    public class UsersReportData
    {
        public int TotalUsers { get; set; }
        public Dictionary<string, int> UsersByType { get; set; }
        public Dictionary<string, int> UsersByStatus { get; set; }
        public Dictionary<DateTime, int> NewUsersPerDay { get; set; }
    }

}
