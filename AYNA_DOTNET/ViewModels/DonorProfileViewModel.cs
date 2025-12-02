using System.ComponentModel.DataAnnotations;

namespace Ayna.ViewModels.DonorVMs
{
    public class DonorProfileViewModel
    {
        public int DonorId { get; set; }

        [Display(Name = "الاسم الأول")]
        public string DonorFirstName { get; set; }

        [Display(Name = "اسم العائلة")]
        public string DonorLastName { get; set; }

        [Display(Name = "البريد الإلكتروني")]
        public string UserEmail { get; set; }

        [Display(Name = "رقم الهاتف")]
        public string UserPhone { get; set; }

        [Display(Name = "تاريخ التسجيل")]
        public DateTime? AddedAt { get; set; }
    }
}
