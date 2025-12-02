namespace Ayna.ViewModels.FarmerVMs
{
    public class MonthlySalesTrend
    {
        public int Month { get; set; }
        public int Count { get; set; }
        public decimal Revenue { get; set; }

        /// <summary>
        /// Gets the Arabic month name
        /// </summary>
        public string MonthName =>
            Month switch
            {
                1 => "يناير",
                2 => "فبراير",
                3 => "مارس",
                4 => "أبريل",
                5 => "مايو",
                6 => "يونيو",
                7 => "يوليو",
                8 => "أغسطس",
                9 => "سبتمبر",
                10 => "أكتوبر",
                11 => "نوفمبر",
                12 => "ديسمبر",
                _ => "غير معروف"
            };
    }
}
