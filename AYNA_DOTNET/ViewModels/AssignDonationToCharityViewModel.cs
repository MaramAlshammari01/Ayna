using System.ComponentModel.DataAnnotations;

namespace Ayna.ViewModels.FarmerVMs
{
    public class AssignDonationToCharityViewModel
    {
        [Required(ErrorMessage = "الجمعية الخيرية مطلوبة")]
        [Display(Name = "الجمعية الخيرية")]
        public int CharId { get; set; }

        [Required(ErrorMessage = "مدة الاستلام مطلوبة")]
        [Display(Name = "مدة الاستلام")]
        public string PickupDuration { get; set; }

        /// <summary>
        /// Available pickup durations
        /// </summary>
        public static readonly string[] PickupDurations = { "Same Day", "1-2 Days", "3-5 Days" };
    }
}
