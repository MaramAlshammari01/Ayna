namespace Ayna.ViewModels.DonorVMs
{
    public class CartItemDto
    {
        public int BasketId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string FarmerName { get; set; }
        public List<string> Crops { get; set; } = new();
    }
}
