using System.ComponentModel.DataAnnotations;

namespace Ayna.ViewModels.AdminVMs
{
    public class UpdateUserStatusViewModel
    {
        [Required(ErrorMessage = "حالة المستخدم مطلوبة")]
        [RegularExpression("^(Active|Inactive|Banned)$", ErrorMessage = "حالة المستخدم غير صالحة")]
        [Display(Name = "حالة المستخدم")]
        public string UserStatus { get; set; }
    }
}
