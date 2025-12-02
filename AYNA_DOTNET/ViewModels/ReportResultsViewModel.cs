namespace Ayna.ViewModels.AdminVMs
{
    public class ReportResultsViewModel
    {
        public string ReportType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public object ReportData { get; set; }
    }
}
