using Ayna.ViewModels.Validation;
using System.ComponentModel.DataAnnotations;

namespace Ayna.ViewModels.CharityVMs
{
    public class UpdateCharityProfileViewModel
    {
        [Required(ErrorMessage = "اسم الجمعية مطلوب")]
        [MaxLength(150, ErrorMessage = "اسم الجمعية يجب ألا يتجاوز 150 حرف")]
        [Display(Name = "اسم الجمعية")]
        public string CharName { get; set; }

        [Required(ErrorMessage = "جهة الاتصال مطلوبة")]
        [MaxLength(100, ErrorMessage = "جهة الاتصال يجب ألا يتجاوز 100 حرف")]
        [Display(Name = "جهة الاتصال")]
        public string CharContact { get; set; }

        [Required(ErrorMessage = "رقم السجل التجاري مطلوب")]
        [CommercialRegistration(ErrorMessage = "رقم السجل التجاري يجب أن يكون 10 أرقام")]
        [Display(Name = "رقم السجل التجاري")]
        public string CharCr { get; set; }

        [Required(ErrorMessage = "الموقع مطلوب")]
        [MaxLength(255, ErrorMessage = "الموقع يجب ألا يتجاوز 255 حرف")]
        [Display(Name = "الموقع")]
        public string CharLocation { get; set; }

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
