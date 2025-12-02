using System.ComponentModel.DataAnnotations;

namespace Ayna.ViewModels.AdminVMs
{
    public class GenerateReportViewModel
    {
        [Required(ErrorMessage = "نوع التقرير مطلوب")]
        [RegularExpression("^(users|donations|orders|financial)$", ErrorMessage = "نوع التقرير غير صالح")]
        [Display(Name = "نوع التقرير")]
        public string ReportType { get; set; }

        [Required(ErrorMessage = "تاريخ البداية مطلوب")]
        [DataType(DataType.Date)]
        [Display(Name = "تاريخ البداية")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "تاريخ النهاية مطلوب")]
        [DataType(DataType.Date)]
        [Display(Name = "تاريخ النهاية")]
        public DateTime EndDate { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (EndDate < StartDate)
            {
                yield return new ValidationResult(
                    "تاريخ النهاية يجب أن يكون بعد تاريخ البداية",
                    new[] { nameof(EndDate) }
                );
            }

            if ((EndDate - StartDate).TotalDays > 365)
            {
                yield return new ValidationResult(
                    "الفترة الزمنية يجب ألا تتجاوز سنة واحدة",
                    new[] { nameof(EndDate) }
                );
            }
        }
    }
}
