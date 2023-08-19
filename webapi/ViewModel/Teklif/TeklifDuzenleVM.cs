namespace webapi.ViewModel.Teklif
{
    public class TeklifDuzenleVM
    {
        public int Id { get; set; }
        public int UrunId { get; set; }
        public string UrunAdi { get; set; }
        public string UrunSahibi { get; set; }
        public decimal TeklifDegeri { get; set; }
    }
}
