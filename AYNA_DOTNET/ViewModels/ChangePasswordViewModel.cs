using System.ComponentModel.DataAnnotations;

namespace Ayna.ViewModels.AuthVMs
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "كلمة المرور الحالية مطلوبة")]
        [DataType(DataType.Password)]
        [Display(Name = "كلمة المرور الحالية")]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "كلمة المرور الجديدة مطلوبة")]
        [StringLength(255, MinimumLength = 8, ErrorMessage = "كلمة المرور يجب أن تكون بين 8 و 255 حرف")]
        [DataType(DataType.Password)]
        [Display(Name = "كلمة المرور الجديدة")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "تأكيد كلمة المرور الجديدة مطلوب")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "كلمة المرور الجديدة وتأكيدها غير متطابقين")]
        [Display(Name = "تأكيد كلمة المرور الجديدة")]
        public string ConfirmNewPassword { get; set; }
    }
}
