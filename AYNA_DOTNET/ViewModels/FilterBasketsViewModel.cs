using System.ComponentModel.DataAnnotations;

namespace Ayna.ViewModels.DonorVMs
{
    public class FilterBasketsViewModel
    {
        [MaxLength(255, ErrorMessage = "البحث يجب ألا يتجاوز 255 حرف")]
        public string Search { get; set; }

        [MaxLength(100, ErrorMessage = "نوع المحصول يجب ألا يتجاوز 100 حرف")]
        public string Type { get; set; }

        [RegularExpression("^(0-50|50-100|100\\+)$", ErrorMessage = "نطاق السعر غير صالح")]
        public string PriceRange { get; set; }
    }
}
