using System.ComponentModel.DataAnnotations;

namespace Ayna.ViewModels.FarmerVMs
{
    public class BasketCropItem
    {
        [Required(ErrorMessage = "معرف المحصول مطلوب")]
        [Display(Name = "المحصول")]
        public int CropId { get; set; }

        [Required(ErrorMessage = "الكمية مطلوبة")]
        [Range(1, int.MaxValue, ErrorMessage = "الكمية يجب أن تكون على الأقل 1")]
        [Display(Name = "الكمية")]
        public int Quantity { get; set; }
    }
}
