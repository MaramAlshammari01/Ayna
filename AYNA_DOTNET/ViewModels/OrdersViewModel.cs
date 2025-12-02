namespace Ayna.ViewModels.CharityVMs
{
    public class OrdersViewModel
    {
        public List<OrderDto> Orders { get; set; } = new();
        public string FilterStatus { get; set; }
    }
}
