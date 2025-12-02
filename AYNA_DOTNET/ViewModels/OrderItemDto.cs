namespace Ayna.ViewModels.CharityVMs
{
    public class OrderItemDto
    {
        public string BasketName { get; set; }
        public int? Quantity { get; set; }
        public decimal? Price { get; set; }
        public decimal? TotalPrice { get; set; }
    }
}
