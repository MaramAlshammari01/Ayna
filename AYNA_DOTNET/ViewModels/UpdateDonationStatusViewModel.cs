using Ayna.Support;
using System.ComponentModel.DataAnnotations;

namespace Ayna.ViewModels.FarmerVMs
{
    public class UpdateDonationStatusViewModel
    {
        [Required(ErrorMessage = "حالة التبرع مطلوبة")]
        [Display(Name = "حالة التبرع")]
        public string DonStatus { get; set; }

        /// <summary>
        /// Validates that the status is one of the allowed values
        /// </summary>
        public bool IsValidStatus()
        {
            return Constants.DONATION_STATUSES.Contains(DonStatus);
        }
    }
}
