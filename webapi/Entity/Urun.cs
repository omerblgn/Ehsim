namespace webapi.Entity
{
    public class Urun
    {
        public int Id { get; set; }
        public string Adi { get; set; }
        public string Aciklama { get; set; }
        public string Ebat { get; set; }
        public decimal Fiyat { get; set; }
        public string ParaBirimi { get; set; }
        public string Tedarikci { get; set; }
        public int Kdv { get; set; }
        public int Kategori { get; set; }
        public bool IsDeleted { get; set; }
        public int Creator { get; set; } = 1;
    }
}
