using System.ComponentModel.DataAnnotations;

namespace webapi.ViewModel.Urun
{
    public class UrunGridVM
    {
        public int Id { get; set; }
        public string Adi { get; set; }
        public string Aciklama { get; set; }
        public string Ebat { get; set; }
        public decimal Fiyat { get; set; }
        public string ParaBirimi { get; set; }
        public string Tedarikci { get; set; }
        public int Kdv { get; set; }
        public string Kategori { get; set; }
        public string UrunSahibi { get; set; }
    }
}
