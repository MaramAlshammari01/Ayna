namespace Ayna.ViewModels.DonorVMs
{
    public class BasketsViewModel
    {
        public List<BasketDto> Baskets { get; set; } = new();
        public List<CharityDto> Charities { get; set; } = new();
        public string SearchQuery { get; set; }
        public string TypeFilter { get; set; }
        public string PriceRangeFilter { get; set; }
        public int CartCount { get; set; }
    }
}
