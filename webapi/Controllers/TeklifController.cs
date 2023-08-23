using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webapi.Base.Base;
using webapi.Base.Base.Grid;
using webapi.Data.Interface;
using webapi.Entity;
using webapi.Helper.Base;
using webapi.ViewModel.General.Grid;
using webapi.ViewModel.Musteri;
using webapi.ViewModel.Teklif;
using webapi.ViewModel.Urun;

namespace webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeklifController : BaseWebApiController
    {
        public TeklifController(IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
        }


        [HttpPost("Create")]
        public ApiResult Create([FromBody] TeklifCreateVM dataVM)
        {
            if (!ModelState.IsValid)
                return new ApiResult { Result = false, Message = "Form'da doldurulmayan alanlar mevcut,lütfen doldurun." };

            int teklifNo;
            bool isUnique = false;

            do
            {
                // Rastgele sayı oluştur
                Random random = new Random();
                teklifNo = random.Next(100000, 1000000); // Örnek aralık (100000 ile 999999 arası)

                // Oluşturulan sayı veritabanında benzersiz mi kontrol et
                var existingTeklif = _unitOfWork.GetContext().Set<Teklif>().FirstOrDefault(t => t.TeklifNo == teklifNo);
                if (existingTeklif == null)
                {
                    isUnique = true; // Benzersiz sayı bulundu
                }

            } while (!isUnique);

            var data = new Teklif()
            {
                TeklifNo = teklifNo,
                TeklifTarihi = DateTime.Now,
                TeklifSuresi = dataVM.TeklifSuresi,
                MusteriId = 1,
                IskontoOrani = dataVM.IskontoOrani,
                ToplamFiyatTRY = dataVM.ToplamFiyatTRY,
                ToplamFiyatUSD = dataVM.ToplamFiyatUSD,
                ToplamFiyatEUR = dataVM.ToplamFiyatEUR,
                TeklifItems = dataVM.TeklifItems.Select(item => new TeklifItem
                {
                    TeklifId = item.TeklifId,
                    UrunId = item.UrunId,
                    Adet = item.Adet,
                    BirimFiyat = item.BirimFiyat,
                    ParaBirimi = item.ParaBirimi
                }).ToList()
            };

            _unitOfWork.GetContext().Set<Teklif>().Add(data);
            _unitOfWork.SaveChanges();
            return new ApiResult { Result = true };
        }

        [HttpPost("GetTekliflerim")]
        public ApiResult<GridResultModel<TekliflerimGridVM>> GetTekliflerim()
        {
            var query = _unitOfWork.GetContext().Set<Teklif>()
                .Where(t => t.MusteriId == 1)   //sadece bir tane müşteri olduğu için otomatik olarak o müşterinin id'si (1) veriliyor
                .Select(teklif => new TekliflerimGridVM
                {
                    Id = teklif.Id,
                    TeklifNo = teklif.TeklifNo,
                    TeklifTarihi = teklif.TeklifTarihi,
                    TeklifSuresi = teklif.TeklifSuresi,
                    MusteriId = teklif.MusteriId,
                    IskontoOrani = teklif.IskontoOrani,
                    ToplamFiyatTRY = teklif.ToplamFiyatTRY,
                    ToplamFiyatUSD = teklif.ToplamFiyatUSD,
                    ToplamFiyatEUR = teklif.ToplamFiyatEUR,
                    TeklifItems = (List<TeklifItemGridVM>)teklif.TeklifItems.Select(item => new TeklifItemGridVM
                    {
                        Id = item.Id,
                        UrunAdi = _unitOfWork.GetContext().Set<Urun>()
                            .Where(u => u.Id == item.UrunId)
                            .Select(u => u.Adi)
                            .FirstOrDefault(),
                        Adet = item.Adet,
                        BirimFiyat = item.BirimFiyat,
                        ParaBirimi = item.ParaBirimi,
                    }),
                }).ToList();
            var rest = query.ToDataListRequest(Request.ToRequestFilter());

            return new ApiResult<GridResultModel<TekliflerimGridVM>> { Data = rest, Result = true };
        }

        [HttpGet("Delete")]
        public ApiResult Delete(int id)
        {
            var teklif = _unitOfWork.GetContext().Set<Teklif>().FirstOrDefault(t => t.Id == id);

            if (teklif == null)
            {
                return new ApiResult { Result = false, Message = "Belirtilen teklif bulunamadı." };
            }

            _unitOfWork.GetContext().Set<Teklif>().Remove(teklif);
            _unitOfWork.SaveChanges();
            return new ApiResult { Result = true };
        }
    }
}
