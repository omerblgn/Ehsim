using System.Reflection.Metadata.Ecma335;

namespace webapi.Entity
{
    public class Teklif
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
        public List<TeklifItem> TeklifItems { get; set; }
    }
    public class TeklifItem
    {
        public int Id { get; set; }
        public int TeklifId { get; set; }
        public int UrunId { get; set; }
        public int Adet { get; set; }
        public decimal BirimFiyat { get; set; }
        public string ParaBirimi { get; set; }
    }
}
