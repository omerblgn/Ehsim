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

            var data = new Teklif()
            {
                UrunId = dataVM.UrunId,
                MusteriId = dataVM.MusteriId,
                TeklifDegeri = dataVM.TeklifDegeri,
            };

            _unitOfWork.GetContext().Set<Teklif>().Add(data);
            _unitOfWork.SaveChanges();
            return new ApiResult { Result = true };
        }

        [HttpPost("TeklifKontrol")]
        public ApiResult<int> TeklifKontrol(int id)
        {
            var teklif = _unitOfWork.GetContext().Set<Teklif>().FirstOrDefault(t => t.UrunId == id && t.MusteriId == 1);
            if (teklif == null)
            {
                return new ApiResult<int> { Data = 0, Result = true, Message = "Teklif yapılmadı" };
            }
            else
            {
                return new ApiResult<int> { Data = 1, Result = true, Message = "Teklif yapıldı" };
            }
        }

        [HttpPost("GetTekliflerim")]
        public ApiResult<GridResultModel<TekliflerimGridVM>> GetTekliflerim()
        {
            var query = _unitOfWork.GetContext().Set<Teklif>()
                .Where(t => t.MusteriId == 1)   //sadece bir tane müşteri olduğu için otomatik olarak o müşterinin id'si (1) veriliyor
                .Select(teklif => new TekliflerimGridVM
                {
                    Id = teklif.Id,
                    UrunAdi = _unitOfWork.GetContext().Set<Urun>()
                        .Where(u => u.Id == teklif.UrunId)
                        .Select(u => u.Adi)
                        .FirstOrDefault(),
                    TeklifDegeri = teklif.TeklifDegeri,
                    UrunSahibi = _unitOfWork.GetContext().Set<Urun>()
                        .Where(u => u.Id == teklif.UrunId)
                        .Select(u => _unitOfWork.GetContext().Set<Musteri>()
                            .Where(m => m.Id == u.Creator)
                            .Select(m => m.Adi + " " + m.Soyadi)
                            .FirstOrDefault())
                        .FirstOrDefault()
                }).ToList();
            var rest = query.ToDataListRequest(Request.ToRequestFilter());

            return new ApiResult<GridResultModel<TekliflerimGridVM>> { Data = rest, Result = true };
        }

        [HttpPost("Get")]
        public ApiResult<TeklifDuzenleVM> Get(int id)
        {
            var teklif = _unitOfWork.GetContext().Set<Teklif>().FirstOrDefault(t => t.Id == id);
            TeklifDuzenleVM teklifVM = new TeklifDuzenleVM
            {
                Id = teklif.Id,
                TeklifDegeri = teklif.TeklifDegeri,
                UrunAdi = _unitOfWork.GetContext().Set<Urun>()
                    .Where(u => u.Id == teklif.UrunId)
                    .Select(u => u.Adi)
                    .FirstOrDefault(),
                UrunSahibi = _unitOfWork.GetContext().Set<Musteri>()
                    .Where(m => m.Id == teklif.MusteriId)
                    .Select(m => m.Adi + " " + m.Soyadi)
                    .FirstOrDefault(),
                UrunId = teklif.UrunId
            };

            return new ApiResult<TeklifDuzenleVM> { Data = teklifVM, Result = true };
        }

        [HttpPost("Update")]
        public ApiResult Update([FromBody] TeklifCreateVM dataVM)
        {
            if (!ModelState.IsValid)
                return new ApiResult { Result = false, Message = "Form'da doldurulmayan alanlar mevcut,lütfen doldurun." };
            Teklif data;

            data = _unitOfWork.GetContext().Set<Teklif>().FirstOrDefault(t => t.Id == dataVM.Id);

            if (data == null)
            {
                return new ApiResult { Result = false, Message = "Belirtilen teklif bulunamadı." };
            }

            data.UrunId = dataVM.UrunId;
            data.MusteriId = dataVM.MusteriId;
            data.TeklifDegeri = dataVM.TeklifDegeri;

            _unitOfWork.SaveChanges();
            return new ApiResult { Result = true };
        }

        [HttpPost("GetGelenTeklifler")]
        public ApiResult<GridResultModel<GelenTekliflerGridVM>> GetGelenTeklifler()
        {
            var query = _unitOfWork.GetContext().Set<Teklif>()
                .Where(teklif => _unitOfWork.GetContext().Set<Urun>().Any(u => u.Id == teklif.UrunId && u.Creator == 1))    //sadece bir tane müşteri olduğu için otomatik olarak o müşterinin id'si (1) veriliyor
                .Select(teklif => new GelenTekliflerGridVM
                {
                    Id = teklif.Id,
                    UrunAdi = _unitOfWork.GetContext().Set<Urun>()
                        .Where(u => u.Id == teklif.UrunId)
                        .Select(u => u.Adi)
                        .FirstOrDefault(),
                    TeklifDegeri = teklif.TeklifDegeri,
                    TeklifVeren = _unitOfWork.GetContext().Set<Musteri>()
                        .Where(m => m.Id == teklif.MusteriId)
                        .Select(m => m.Adi + " " + m.Soyadi)
                        .FirstOrDefault()
                }).ToList();
            var rest = query.ToDataListRequest(Request.ToRequestFilter());

            return new ApiResult<GridResultModel<GelenTekliflerGridVM>> { Data = rest, Result = true };
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
