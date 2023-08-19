using System.ComponentModel.DataAnnotations;

namespace webapi.ViewModel.Kategori
{
    public class KategoriCreateVM
    {
        public int Id { get; set; }
        [Required]
        public string Adi { get; set; }
    }
}
