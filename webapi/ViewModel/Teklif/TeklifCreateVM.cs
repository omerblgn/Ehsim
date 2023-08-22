using System.ComponentModel.DataAnnotations;
using webapi.Entity;

namespace webapi.ViewModel.Teklif
{
    public class TeklifCreateVM
    {
        public int Id { get; set; }
        //    [Required]
        //    public int UrunId { get; set; }
        //    [Required]
        //    public int MusteriId { get; set; }
        //    [Required]
        //    public decimal TeklifDegeri { get; set; }

        [Required]
        public int TeklifNo { get; set; }
        [Required]
        public DateTime TeklifTarihi { get; set; }
        [Required]
        public int TeklifSuresi { get; set; }
        [Required]
        public int MusteriId { get; set; }
        [Required]
        public decimal IskontoOrani { get; set; }
        [Required]
        public decimal ToplamFiyat { get; set; }
        [Required]
        public List<TeklifItem> TeklifItems { get; set; }
    }
}
