namespace Ayna.ViewModels.AdminVMs
{
    public class UsersIndexViewModel
    {
        public List<UserListItemViewModel> Users { get; set; } = new();
        public string FilterType { get; set; }
        public string FilterStatus { get; set; }
    }
}
