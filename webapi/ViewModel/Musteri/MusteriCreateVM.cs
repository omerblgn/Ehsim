using System.ComponentModel.DataAnnotations;

namespace webapi.ViewModel.Musteri
{
    public class MusteriCreateVM
    {
        public int Id { get; set; }
        [Required]
        public string Adi { get; set; }
        [Required]
        public string Soyadi { get; set; }
        [Required]
        public string TelefonNumarasi { get; set; }
        [Required]
        public string Email { get; set; }
    }
}
