using Ayna.ViewModels.Validation;
using System.ComponentModel.DataAnnotations;

namespace Ayna.ViewModels.FarmerVMs
{
    public class UpdateFarmerProfileViewModel
    {
        [Required(ErrorMessage = "الاسم الأول مطلوب")]
        [StringLength(100, ErrorMessage = "الاسم الأول يجب أن لا يتجاوز 100 حرف")]
        [Display(Name = "الاسم الأول")]
        public string FarFirstName { get; set; }

        [Required(ErrorMessage = "اسم العائلة مطلوب")]
        [StringLength(100, ErrorMessage = "اسم العائلة يجب أن لا يتجاوز 100 حرف")]
        [Display(Name = "اسم العائلة")]
        public string FarLastName { get; set; }

        [Required(ErrorMessage = "الموقع مطلوب")]
        [StringLength(500, ErrorMessage = "الموقع يجب أن لا يتجاوز 500 حرف")]
        [Display(Name = "الموقع")]
        public string FarLocation { get; set; }

        [Required(ErrorMessage = "البريد الإلكتروني مطلوب")]
        [EmailAddress(ErrorMessage = "البريد الإلكتروني غير صالح")]
        [StringLength(100, ErrorMessage = "البريد الإلكتروني يجب أن لا يتجاوز 100 حرف")]
        [Display(Name = "البريد الإلكتروني")]
        public string UserEmail { get; set; }

        [Required(ErrorMessage = "رقم الهاتف مطلوب")]
        [SaudiPhoneNumber(ErrorMessage = "رقم الهاتف يجب أن يكون صالحاً (مثال: +966566193395 أو 0566193395)")]
        [StringLength(20, ErrorMessage = "رقم الهاتف يجب أن لا يتجاوز 20 رقم")]
        [Display(Name = "رقم الهاتف")]
        public string UserPhone { get; set; }

        [DataType(DataType.Password)]
        [StringLength(255, MinimumLength = 8, ErrorMessage = "كلمة المرور يجب أن تكون بين 8 و 255 حرف")]
        [Display(Name = "كلمة المرور الجديدة")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "كلمة المرور وتأكيد كلمة المرور غير متطابقتين")]
        [Display(Name = "تأكيد كلمة المرور")]
        public string PasswordConfirmation { get; set; }
    }
}
