using System.ComponentModel.DataAnnotations;

namespace Ayna.ViewModels.CharityVMs
{
    public class CreateDonationRequestViewModel
    {
        [Required(ErrorMessage = "وصف الطلب مطلوب")]
        [MaxLength(255, ErrorMessage = "وصف الطلب يجب ألا يتجاوز 255 حرف")]
        [Display(Name = "وصف الطلب")]
        public string ReqDonation { get; set; }

        [Required(ErrorMessage = "مدة الاستلام مطلوبة")]
        [MaxLength(100, ErrorMessage = "مدة الاستلام يجب ألا يتجاوز 100 حرف")]
        [Display(Name = "مدة الاستلام المتوقعة")]
        public string PickupDuration { get; set; }
    }
}
