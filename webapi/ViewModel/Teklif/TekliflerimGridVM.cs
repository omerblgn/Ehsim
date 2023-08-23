using webapi.Entity;

namespace webapi.ViewModel.Teklif
{
    public class TekliflerimGridVM
    {
        public int Id { get; set; }
        public int TeklifNo { get; set; }
        public DateTime TeklifTarihi { get; set; }
        public int TeklifSuresi { get; set; }
        public int MusteriId { get; set; }
        public decimal IskontoOrani { get; set; }
        public decimal ToplamFiyatTRY { get; set; }
        public decimal ToplamFiyatUSD { get; set; }
        public decimal ToplamFiyatEUR { get; set; }
        public List<TeklifItemGridVM> TeklifItems { get; set; }
    }
    public class TeklifItemGridVM
    {
        public int Id { get; set; }
        public int TeklifId { get; set; }
        public string UrunAdi { get; set; }
        public int Adet { get; set; }
        public decimal BirimFiyat { get; set; }
        public string ParaBirimi { get; set; }
    }
}
