using System.ComponentModel.DataAnnotations;

namespace Ayna.ViewModels.FarmerVMs
{
    public class ReportExportOptions
    {
        [Display(Name = "تاريخ البداية")]
        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; }

        [Display(Name = "تاريخ النهاية")]
        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }

        [Display(Name = "نوع التقرير")]
        public ReportType ReportType { get; set; }

        [Display(Name = "صيغة التصدير")]
        public ExportFormat ExportFormat { get; set; }
    }

    /// <summary>
    /// Report types
    /// </summary>
    public enum ReportType
    {
        [Display(Name = "جرد المحاصيل")]
        CropInventory = 1,

        [Display(Name = "تقرير الأداء")]
        Performance = 2,

        [Display(Name = "تقرير التبرعات")]
        Donations = 3,

        [Display(Name = "تقرير المبيعات")]
        Sales = 4
    }

    /// <summary>
    /// Export formats
    /// </summary>
    public enum ExportFormat
    {
        [Display(Name = "CSV")]
        CSV = 1,

        [Display(Name = "Excel")]
        Excel = 2,

        [Display(Name = "PDF")]
        PDF = 3
    }
}
