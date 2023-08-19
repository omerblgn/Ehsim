using System.ComponentModel.DataAnnotations;

namespace webapi.ViewModel.Urun
{
    public class UrunCreateVM
    {
        public int Id { get; set; }
        [Required]
        public string Adi { get; set; }
        public string Aciklama { get; set; }
        public string Ebat { get; set; }
        [Required]
        public decimal Fiyat { get; set; }
        [Required]
        public string ParaBirimi { get; set; }
        public string Tedarikci { get; set; }
        [Required]
        public int Kdv { get; set; }
        [Required]
        public int Kategori { get; set; }
    }
}
