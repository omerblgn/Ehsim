using Microsoft.AspNetCore.Mvc;
using webapi.Base.Base;
using webapi.Base.Base.Grid;
using webapi.Entity;
using webapi.Helper.Base;
using webapi.ViewModel.General.Grid;
using webapi.ViewModel.Urun;

namespace webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UrunController : BaseWebApiController
    {
        public UrunController(IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
        }

        [HttpPost("CreateOrUpdate")]
        public ApiResult CreateOrUpdate([FromBody] UrunCreateVM dataVM)
        {
            if (!ModelState.IsValid)
                return new ApiResult { Result = false, Message = "Form'da doldurulmayan alanlar mevcut,lütfen doldurun." };
            Urun data;
            if (dataVM.Id > 0)
            {
                data = _unitOfWork.GetContext().Set<Urun>().FirstOrDefault(u => u.Id == dataVM.Id);
                data.Adi = dataVM.Adi;
                data.Aciklama = dataVM.Aciklama;
                data.Ebat = dataVM.Ebat;
                data.Fiyat = dataVM.Fiyat;
                data.ParaBirimi = dataVM.ParaBirimi;
                data.Tedarikci = dataVM.Tedarikci;
                data.Kdv = dataVM.Kdv;
                data.Kategori = dataVM.Kategori;
            }
            else
            {
                data = new Urun()
                {
                    Adi = dataVM.Adi,
                    Aciklama = dataVM.Aciklama,
                    Ebat = dataVM.Ebat,
                    Fiyat = dataVM.Fiyat,
                    ParaBirimi = dataVM.ParaBirimi,
                    Tedarikci = dataVM.Tedarikci,
                    Kdv = dataVM.Kdv,
                    Kategori = dataVM.Kategori,
                };
                if (_unitOfWork.GetContext().Set<Urun>().Any(x => x == data))
                {
                    return new ApiResult { Result = false, Message = "Daha önce eklenmiş" };
                }
                _unitOfWork.GetContext().Set<Urun>().Add(data);
            }

            _unitOfWork.SaveChanges();
            return new ApiResult { Result = true };
        }

        [HttpGet("Delete")]
        public ApiResult Delete(int id)
        {
            var data = _unitOfWork.GetContext().Set<Urun>().FirstOrDefault(u => u.Id == id);

            if (data == null)
            {
                return new ApiResult { Result = false, Message = "Belirtilen ürün bulunamadı." };
            }

            //_unitOfWork.GetContext().Set<Urun>().SoftDelete(data.Id);
            data.IsDeleted = true;
            _unitOfWork.SaveChanges();
            return new ApiResult { Result = true };
        }

        [HttpPost("GetGrid")]
        public ApiResult<GridResultModel<UrunGridVM>> GetGrid()
        {
            var query = _unitOfWork.GetContext().Set<Urun>()
            .Where(x => x.IsDeleted == false)
            .Select(x => new UrunGridVM
            {
                Id = x.Id,
                Adi = x.Adi,
                Aciklama = x.Aciklama,
                Ebat = x.Ebat,
                Fiyat = x.Fiyat,
                ParaBirimi = x.ParaBirimi,
                Tedarikci = x.Tedarikci,
                Kdv = x.Kdv,
                Kategori = _unitOfWork.GetContext().Set<Kategori>()
                    .Where(k => k.Id == x.Kategori)
                    .Select(k => k.Adi)
                    .FirstOrDefault(),
                UrunSahibi = _unitOfWork.GetContext().Set<Musteri>()
                            .Where(m => m.Id == x.Creator)
                            .Select(m => m.Adi + " " + m.Soyadi)
                            .FirstOrDefault()
            });
            var rest = query.ToDataListRequest(Request.ToRequestFilter());

            return new ApiResult<GridResultModel<UrunGridVM>> { Data = rest, Result = true };
        }

        [HttpPost("Get")]
        public ApiResult<Urun> Get(int id)
        {
            var urun = _unitOfWork.GetContext().Set<Urun>().FirstOrDefault(u => u.Id == id);
            if (urun == null)
            {
                return new ApiResult<Urun> { Result = false, Message = "Ürün bulunamadı." };
            }
            Urun musteriVM = new Urun
            {
                Id = urun.Id,
                Adi = urun.Adi,
                Aciklama = urun.Aciklama,
                Ebat = urun.Ebat,
                Fiyat = urun.Fiyat,
                ParaBirimi = urun.ParaBirimi,
                Tedarikci = urun.Tedarikci,
                Kdv = urun.Kdv,
                Kategori = urun.Kategori,
            };
            return new ApiResult<Urun> { Data = musteriVM, Result = true };
        }
    }
}
