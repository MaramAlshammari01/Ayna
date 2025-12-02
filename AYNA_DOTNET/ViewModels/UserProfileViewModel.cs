using Ayna.ViewModels.Validation;
using System.ComponentModel.DataAnnotations;

namespace Ayna.ViewModels.AuthVMs
{
    public class UserProfileViewModel
    {
        public int UserId { get; set; }

        [Required(ErrorMessage = "البريد الإلكتروني مطلوب")]
        [EmailAddress(ErrorMessage = "البريد الإلكتروني غير صحيح")]
        [Display(Name = "البريد الإلكتروني")]
        public string UserEmail { get; set; }

        [Required(ErrorMessage = "رقم الهاتف مطلوب")]
        [SaudiPhoneNumber(ErrorMessage = "رقم الهاتف غير صحيح")]
        [Display(Name = "رقم الهاتف")]
        public string UserPhone { get; set; }

        [Display(Name = "نوع المستخدم")]
        public string UserType { get; set; }

        [Display(Name = "حالة المستخدم")]
        public string UserStatus { get; set; }

        [Display(Name = "تاريخ الإنشاء")]
        public DateTime? AddedAt { get; set; }

        // Farmer-specific fields
        [StringLength(100, ErrorMessage = "الاسم الأول يجب أن لا يتجاوز 100 حرف")]
        [Display(Name = "الاسم الأول")]
        public string FarmerFirstName { get; set; }

        [StringLength(100, ErrorMessage = "اسم العائلة يجب أن لا يتجاوز 100 حرف")]
        [Display(Name = "اسم العائلة")]
        public string FarmerLastName { get; set; }

        [StringLength(255, ErrorMessage = "الموقع يجب أن لا يتجاوز 255 حرف")]
        [Display(Name = "الموقع")]
        public string FarmerLocation { get; set; }

        // Donor-specific fields
        [StringLength(100, ErrorMessage = "الاسم الأول يجب أن لا يتجاوز 100 حرف")]
        [Display(Name = "الاسم الأول")]
        public string DonorFirstName { get; set; }

        [StringLength(100, ErrorMessage = "اسم العائلة يجب أن لا يتجاوز 100 حرف")]
        [Display(Name = "اسم العائلة")]
        public string DonorLastName { get; set; }

        // Charity-specific fields
        [StringLength(150, ErrorMessage = "اسم الجمعية يجب أن لا يتجاوز 150 حرف")]
        [Display(Name = "اسم الجمعية")]
        public string CharityName { get; set; }

        [StringLength(100, ErrorMessage = "السجل التجاري يجب أن لا يتجاوز 100 حرف")]
        [Display(Name = "السجل التجاري")]
        public string CharityCR { get; set; }

        [StringLength(255, ErrorMessage = "الموقع يجب أن لا يتجاوز 255 حرف")]
        [Display(Name = "الموقع")]
        public string CharityLocation { get; set; }

        [StringLength(100, ErrorMessage = "جهة الاتصال يجب أن لا يتجاوز 100 حرف")]
        [Display(Name = "جهة الاتصال")]
        public string CharityContact { get; set; }
    }
}
