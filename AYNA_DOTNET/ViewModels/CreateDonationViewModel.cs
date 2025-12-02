using System.ComponentModel.DataAnnotations;

namespace Ayna.ViewModels.FarmerVMs
{
    public class CreateDonationViewModel
    {
        [Required(ErrorMessage = "وصف التبرع مطلوب")]
        [StringLength(1000, ErrorMessage = "وصف التبرع يجب أن لا يتجاوز 1000 حرف")]
        [Display(Name = "وصف التبرع")]
        public string DonDescription { get; set; }

        [Required(ErrorMessage = "الجمعية الخيرية مطلوبة")]
        [Display(Name = "الجمعية الخيرية")]
        public int CharId { get; set; }

        [Required(ErrorMessage = "مدة الاستلام مطلوبة")]
        [Display(Name = "مدة الاستلام")]
        public string PickupDuration { get; set; }

        [Required(ErrorMessage = "يجب اختيار المحاصيل")]
        [MinLength(1, ErrorMessage = "يجب اختيار محصول واحد على الأقل")]
        [Display(Name = "المحاصيل")]
        public List<DonationCropItem> Crops { get; set; } = new();

        /// <summary>
        /// Available pickup durations
        /// </summary>
        public static readonly string[] PickupDurations = { "Same Day", "1-2 Days", "3-5 Days" };
    }

}
