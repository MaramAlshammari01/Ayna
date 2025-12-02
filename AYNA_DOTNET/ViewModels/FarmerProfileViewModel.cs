using System.ComponentModel.DataAnnotations;

namespace Ayna.ViewModels.FarmerVMs
{
    public class FarmerProfileViewModel
    {
        public int FarId { get; set; }

        [Display(Name = "الاسم الأول")]
        public string FarFirstName { get; set; }

        [Display(Name = "اسم العائلة")]
        public string FarLastName { get; set; }

        [Display(Name = "الموقع")]
        public string FarLocation { get; set; }

        [Display(Name = "البريد الإلكتروني")]
        public string UserEmail { get; set; }

        [Display(Name = "رقم الهاتف")]
        public string UserPhone { get; set; }

        /// <summary>
        /// Gets the farmer's full name
        /// </summary>
        public string FullName => $"{FarFirstName} {FarLastName}";
    }
}
