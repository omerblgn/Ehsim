using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using webapi.Data.Interface;
using webapi.Entity;
using webapi.Helper.Extensions;
using webapi.ViewModel;

namespace webapi.Base.Base
{
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public class BaseWebApiController : Controller
    {
        public readonly IUnitOfWork _unitOfWork;
        public readonly IHttpContextAccessor _httpContextAccessor;
        
        public BaseWebApiController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork = httpContextAccessor.GetRegisterService<IUnitOfWork>();
        }
    }
}
