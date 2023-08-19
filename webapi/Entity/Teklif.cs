namespace webapi.Entity
{
    public class Teklif
    {
        public int Id { get; set; }
        public int UrunId { get; set; }
        public int MusteriId { get; set; }
        public decimal TeklifDegeri { get; set; }
    }
}
