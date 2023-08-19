using System.ComponentModel.DataAnnotations;

namespace webapi.ViewModel.Teklif
{
    public class TeklifCreateVM
    {
        public int Id { get; set; }
        [Required]
        public int UrunId { get; set; }
        [Required]
        public int MusteriId { get; set; }
        [Required]
        public decimal TeklifDegeri { get; set; }
    }
}
