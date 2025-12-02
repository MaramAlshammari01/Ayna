using System.ComponentModel.DataAnnotations;

namespace Ayna.ViewModels.DonorVMs
{
    public class UpdateQuantityViewModel
    {
        [Required(ErrorMessage = "الكمية مطلوبة")]
        [Range(1, int.MaxValue, ErrorMessage = "الكمية يجب أن تكون أكبر من صفر")]
        public int Quantity { get; set; }
    }
}
