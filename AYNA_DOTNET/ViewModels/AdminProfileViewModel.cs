using System.ComponentModel.DataAnnotations;

namespace Ayna.ViewModels.AdminVMs
{
    /// <summary>
    /// ViewModel for Admin profile management
    /// </summary>
    public class AdminProfileViewModel
    {
        public int UserId { get; set; }

        [Required(ErrorMessage = "البريد الإلكتروني مطلوب")]
        [EmailAddress(ErrorMessage = "البريد الإلكتروني غير صحيح")]
        [Display(Name = "البريد الإلكتروني")]
        public string UserEmail { get; set; }

        [Required(ErrorMessage = "رقم الهاتف مطلوب")]
        [Phone(ErrorMessage = "رقم الهاتف غير صحيح")]
        [Display(Name = "رقم الهاتف")]
        public string UserPhone { get; set; }

        [Display(Name = "نوع المستخدم")]
        public string UserType { get; set; }

        [Display(Name = "الحالة")]
        public string UserStatus { get; set; }

        [Display(Name = "تاريخ الإضافة")]
        public DateTime? AddedAt { get; set; }

        // Password fields (optional - only if user wants to change password)
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage = "كلمة المرور يجب أن تكون 8 أحرف على الأقل")]
        [Display(Name = "كلمة المرور الجديدة (اختياري)")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "كلمة المرور وتأكيدها غير متطابقين")]
        [Display(Name = "تأكيد كلمة المرور الجديدة")]
        public string PasswordConfirmation { get; set; }
    }
}