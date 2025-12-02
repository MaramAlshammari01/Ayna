using System.ComponentModel.DataAnnotations;

namespace Ayna.ViewModels.CharityVMs
{
    public class CharityProfileViewModel
    {
        public int CharId { get; set; }

        [Display(Name = "اسم الجمعية")]
        public string CharName { get; set; }

        [Display(Name = "جهة الاتصال")]
        public string CharContact { get; set; }

        [Display(Name = "رقم السجل التجاري")]
        public string CharCr { get; set; }

        [Display(Name = "الموقع")]
        public string CharLocation { get; set; }

        [Display(Name = "البريد الإلكتروني")]
        public string UserEmail { get; set; }

        [Display(Name = "رقم الهاتف")]
        public string UserPhone { get; set; }

        [Display(Name = "تاريخ التسجيل")]
        public DateTime? AddedAt { get; set; }
    }
}
