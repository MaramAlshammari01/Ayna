using Ayna.ViewModels.Validation;
using System.ComponentModel.DataAnnotations;

namespace Ayna.ViewModels.FarmerVMs
{
    public class UpdateCropViewModel
    {
        public int CroId { get; set; }

        [Required(ErrorMessage = "اسم المحصول مطلوب")]
        [StringLength(100, ErrorMessage = "اسم المحصول يجب أن لا يتجاوز 100 حرف")]
        [Display(Name = "اسم المحصول")]
        public string CroName { get; set; }

        [Required(ErrorMessage = "نوع المحصول مطلوب")]
        [StringLength(100, ErrorMessage = "نوع المحصول يجب أن لا يتجاوز 100 حرف")]
        [Display(Name = "نوع المحصول")]
        public string CroType { get; set; }

        [Required(ErrorMessage = "وزن المحصول مطلوب")]
        [Range(0.01, 999999.99, ErrorMessage = "الوزن يجب أن يكون بين 0.01 و 999999.99")]
        [Display(Name = "وزن المحصول")]
        public decimal CroWeight { get; set; }

        [Required(ErrorMessage = "كمية المحصول مطلوبة")]
        [Range(1, int.MaxValue, ErrorMessage = "الكمية يجب أن تكون على الأقل 1")]
        [Display(Name = "كمية المحصول")]
        public int CroQuantity { get; set; }

        [Required(ErrorMessage = "وحدة القياس مطلوبة")]
        [StringLength(50, ErrorMessage = "وحدة القياس يجب أن لا تتجاوز 50 حرف")]
        [Display(Name = "وحدة القياس")]
        public string CroUnit { get; set; }

        [Required(ErrorMessage = "تاريخ الانتهاء مطلوب")]
        [FutureDate(ErrorMessage = "تاريخ الانتهاء يجب أن يكون في المستقبل")]
        [Display(Name = "تاريخ الانتهاء")]
        [DataType(DataType.DateTime)]
        public DateTime ExpiredAt { get; set; }
    }
}
