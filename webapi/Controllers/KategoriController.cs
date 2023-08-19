using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using webapi.Base.Base;
using webapi.Data;
using webapi.Entity;
using webapi.Helper.Base;
using webapi.ViewModel.Teklif;

namespace webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KategoriController : BaseWebApiController
    {
        public KategoriController(IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
        }

        [HttpPost("Get")]
        public ApiResult<List<Kategori>> Get()
        {
            var kategori = _unitOfWork.GetContext().Set<Kategori>().ToList();

            return new ApiResult<List<Kategori>> { Data = kategori, Result = true };
        }
    }
}
