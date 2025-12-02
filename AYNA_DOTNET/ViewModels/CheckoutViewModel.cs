using System.ComponentModel.DataAnnotations;

namespace Ayna.ViewModels.DonorVMs
{
    public class CheckoutViewModel
    {
        [Required(ErrorMessage = "يجب اختيار جمعية خيرية")]
        public int? CharId { get; set; }

        [Required(ErrorMessage = "يجب اختيار طريقة الدفع")]
        [MaxLength(50, ErrorMessage = "طريقة الدفع يجب ألا تتجاوز 50 حرف")]
        public string PayMethod { get; set; }
    }
}
