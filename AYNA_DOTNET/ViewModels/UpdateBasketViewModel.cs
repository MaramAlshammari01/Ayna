using System.ComponentModel.DataAnnotations;

namespace Ayna.ViewModels.FarmerVMs
{
    public class UpdateBasketViewModel
    {
        public int BasId { get; set; }

        [Required(ErrorMessage = "محتوى السلة مطلوب")]
        [StringLength(500, ErrorMessage = "محتوى السلة يجب أن لا يتجاوز 500 حرف")]
        [Display(Name = "محتوى السلة")]
        public string BasContent { get; set; }

        [Required(ErrorMessage = "سعر السلة مطلوب")]
        [Range(0.01, 999999.99, ErrorMessage = "السعر يجب أن يكون بين 0.01 و 999999.99")]
        [Display(Name = "سعر السلة")]
        public decimal BasPrice { get; set; }

        [Required(ErrorMessage = "كمية السلة مطلوبة")]
        [Range(1, int.MaxValue, ErrorMessage = "الكمية يجب أن تكون على الأقل 1")]
        [Display(Name = "كمية السلة")]
        public int BasQty { get; set; }
    }
}
