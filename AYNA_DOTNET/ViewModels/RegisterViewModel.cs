using Ayna.ViewModels.Validation;
using System.ComponentModel.DataAnnotations;

namespace Ayna.ViewModels.AuthVMs
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "الاسم الأول مطلوب")]
        [StringLength(100, ErrorMessage = "الاسم الأول يجب أن لا يتجاوز 100 حرف")]
        [Display(Name = "الاسم الأول")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "اسم العائلة مطلوب")]
        [StringLength(100, ErrorMessage = "اسم العائلة يجب أن لا يتجاوز 100 حرف")]
        [Display(Name = "اسم العائلة")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "البريد الإلكتروني مطلوب")]
        [EmailAddress(ErrorMessage = "البريد الإلكتروني غير صحيح")]
        [StringLength(100, ErrorMessage = "البريد الإلكتروني يجب أن لا يتجاوز 100 حرف")]
        [Display(Name = "البريد الإلكتروني")]
        public string UserEmail { get; set; }

        [Required(ErrorMessage = "كلمة المرور مطلوبة")]
        [StringLength(255, MinimumLength = 8, ErrorMessage = "كلمة المرور يجب أن تكون بين 8 و 255 حرف")]
        [DataType(DataType.Password)]
        [Display(Name = "كلمة المرور")]
        public string UserPassword { get; set; }

        [Required(ErrorMessage = "تأكيد كلمة المرور مطلوب")]
        [DataType(DataType.Password)]
        [Compare("UserPassword", ErrorMessage = "كلمة المرور وتأكيد كلمة المرور غير متطابقين")]
        [Display(Name = "تأكيد كلمة المرور")]
        public string UserPasswordConfirmation { get; set; }

        [Required(ErrorMessage = "رقم الهاتف مطلوب")]
        [SaudiPhoneNumber(ErrorMessage = "رقم الهاتف غير صحيح. مثال: +966566193395 أو 0566193395")]
        [StringLength(20, ErrorMessage = "رقم الهاتف يجب أن لا يتجاوز 20 حرف")]
        [Display(Name = "رقم الهاتف")]
        public string UserPhone { get; set; }

        [Required(ErrorMessage = "نوع المستخدم مطلوب")]
        [Display(Name = "نوع المستخدم")]
        public string UserType { get; set; }

        [Required(ErrorMessage = "الموقع مطلوب")]
        [StringLength(500, ErrorMessage = "الموقع يجب أن لا يتجاوز 500 حرف")]
        [Display(Name = "الموقع")]
        public string Location { get; set; }

        // Charity-specific fields
        [StringLength(255, ErrorMessage = "اسم الجمعية يجب أن لا يتجاوز 255 حرف")]
        [Display(Name = "اسم الجمعية")]
        public string CharName { get; set; }

        [CommercialRegistration(ErrorMessage = "السجل التجاري يجب أن يكون 10 أرقام")]
        [StringLength(100, ErrorMessage = "السجل التجاري يجب أن لا يتجاوز 100 حرف")]
        [Display(Name = "السجل التجاري")]
        public string CharCR { get; set; }

        [Display(Name = "أوافق على الشروط والأحكام")]
        [Range(typeof(bool), "true", "true", ErrorMessage = "يجب الموافقة على الشروط والأحكام")]
        public bool AgreeToTerms { get; set; }
    }
}
