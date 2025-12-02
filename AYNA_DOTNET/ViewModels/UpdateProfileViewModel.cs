using Ayna.ViewModels.Validation;
using System.ComponentModel.DataAnnotations;

namespace Ayna.ViewModels.AdminVMs
{
    public class UpdateProfileViewModel
    {
        [Required(ErrorMessage = "البريد الإلكتروني مطلوب")]
        [EmailAddress(ErrorMessage = "صيغة البريد الإلكتروني غير صحيحة")]
        [MaxLength(100, ErrorMessage = "البريد الإلكتروني يجب ألا يتجاوز 100 حرف")]
        [Display(Name = "البريد الإلكتروني")]
        public string UserEmail { get; set; }

        [Required(ErrorMessage = "رقم الهاتف مطلوب")]
        [SaudiPhoneNumber(ErrorMessage = "رقم الهاتف غير صحيح. مثال: +966566193395 أو 0566193395")]
        [Display(Name = "رقم الهاتف")]
        public string UserPhone { get; set; }

        [MinLength(6, ErrorMessage = "كلمة المرور يجب أن تكون 6 أحرف على الأقل")]
        [DataType(DataType.Password)]
        [Display(Name = "كلمة المرور الجديدة")]
        public string Password { get; set; }

        [Compare(nameof(Password), ErrorMessage = "كلمة المرور وتأكيد كلمة المرور غير متطابقتين")]
        [DataType(DataType.Password)]
        [Display(Name = "تأكيد كلمة المرور")]
        public string PasswordConfirmation { get; set; }
    }
}
