using Ayna.ViewModels.Validation;
using System.ComponentModel.DataAnnotations;

namespace Ayna.ViewModels.DonorVMs
{
    public class UpdateDonorProfileViewModel
    {
        [Required(ErrorMessage = "الاسم الأول مطلوب")]
        [MaxLength(100, ErrorMessage = "الاسم الأول يجب ألا يتجاوز 100 حرف")]
        [Display(Name = "الاسم الأول")]
        public string DonorFirstName { get; set; }

        [Required(ErrorMessage = "اسم العائلة مطلوب")]
        [MaxLength(100, ErrorMessage = "اسم العائلة يجب ألا يتجاوز 100 حرف")]
        [Display(Name = "اسم العائلة")]
        public string DonorLastName { get; set; }

        [Required(ErrorMessage = "البريد الإلكتروني مطلوب")]
        [EmailAddress(ErrorMessage = "صيغة البريد الإلكتروني غير صحيحة")]
        [MaxLength(100, ErrorMessage = "البريد الإلكتروني يجب ألا يتجاوز 100 حرف")]
        [Display(Name = "البريد الإلكتروني")]
        public string UserEmail { get; set; }

        [Required(ErrorMessage = "رقم الهاتف مطلوب")]
        [SaudiPhoneNumber(ErrorMessage = "رقم الهاتف غير صحيح. مثال: +966566193395 أو 0566193395")]
        [Display(Name = "رقم الهاتف")]
        public string UserPhone { get; set; }

        [MinLength(8, ErrorMessage = "كلمة المرور يجب أن تكون 8 أحرف على الأقل")]
        [DataType(DataType.Password)]
        [Display(Name = "كلمة المرور الجديدة")]
        public string Password { get; set; }

        [Compare(nameof(Password), ErrorMessage = "كلمة المرور وتأكيد كلمة المرور غير متطابقتين")]
        [DataType(DataType.Password)]
        [Display(Name = "تأكيد كلمة المرور")]
        public string PasswordConfirmation { get; set; }
    }
}
