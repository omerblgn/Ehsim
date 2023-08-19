using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using webapi.Base.Base;
using webapi.Base.Base.Grid;
using webapi.Data;
using webapi.Entity;
using webapi.Helper.Base;
using webapi.ViewModel.General.Grid;
using webapi.ViewModel.Kategori;
using webapi.ViewModel.Urun;

namespace webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KategoriController : BaseWebApiController
    {
        public KategoriController(IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
        }

        [HttpPost("CreateOrUpdate")]
        public ApiResult CreateOrUpdate([FromBody] KategoriCreateVM dataVM)
        {
            if (!ModelState.IsValid)
                return new ApiResult { Result = false, Message = "Form'da doldurulmayan alanlar mevcut,lütfen doldurun." };
            Kategori data;
            if (dataVM.Id > 0)
            {
                data = _unitOfWork.GetContext().Set<Kategori>().FirstOrDefault(u => u.Id == dataVM.Id);
                data.Adi = dataVM.Adi;
            }
            else
            {
                data = new Kategori()
                {
                    Adi = dataVM.Adi,
                };
                if (_unitOfWork.GetContext().Set<Kategori>().Any(x => x == data))
                {
                    return new ApiResult { Result = false, Message = "Daha önce eklenmiş" };
                }
                _unitOfWork.GetContext().Set<Kategori>().Add(data);
            }

            _unitOfWork.SaveChanges();
            return new ApiResult { Result = true };
        }

        [HttpGet("Delete")]
        public ApiResult Delete(int id)
        {
            var data = _unitOfWork.GetContext().Set<Kategori>().FirstOrDefault(u => u.Id == id);

            if (data == null)
            {
                return new ApiResult { Result = false, Message = "Belirtilen ürün bulunamadı." };
            }

            data.IsDeleted = true;
            _unitOfWork.SaveChanges();
            return new ApiResult { Result = true };
        }

        [HttpPost("GetGrid")]
        public ApiResult<GridResultModel<KategoriGridVM>> GetGrid()
        {
            var query = _unitOfWork.GetContext().Set<Kategori>()
            .Where(x => x.IsDeleted == false)
            .Select(x => new KategoriGridVM
            {
                Id = x.Id,
                Adi = x.Adi
            });
            var rest = query.ToDataListRequest(Request.ToRequestFilter());

            return new ApiResult<GridResultModel<KategoriGridVM>> { Data = rest, Result = true };
        }

        [HttpPost("Get")]
        public ApiResult<object> Get(int? id)
        {
            if (id.HasValue)
            {
                var kategori = _unitOfWork.GetContext().Set<Kategori>().FirstOrDefault(u => u.Id == id);
                KategoriGridVM kategoriVM = new KategoriGridVM
                {
                    Id = kategori.Id,
                    Adi = kategori.Adi,
                };
                return new ApiResult<object> { Data = kategoriVM, Result = true };
            }
            else
            {
                var kategori = _unitOfWork.GetContext().Set<Kategori>()
                    .Where(x => x.IsDeleted == false)
                    .ToList();

                return new ApiResult<object> { Data = kategori, Result = true };
            }
        }
    }
}
