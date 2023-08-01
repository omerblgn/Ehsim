using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using webapi.Base.Base;
using webapi.Base.Base.Grid;
using webapi.Entity;
using webapi.Helper.Base;
using webapi.ViewModel.Customer;
using webapi.ViewModel.General.Grid;

namespace webapi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [AllowAnonymous]
    public class CustomerController : BaseWebApiController
    {
        public Customer[] myCustomer;
        private static readonly string[] Names = new[]
        {
            "Ali","Ahmet","Mehmet","Erol","Murat","Ayse","Sule","Nur","Fatma"
        };

        public CustomerController(IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
        }


        [HttpGet(Name = "CustomerController")]
        public IEnumerable<Customer> Get()
        {
            var a = Enumerable.Range(1, 10).Select(index => new Customer
            {
                Adi = Names[Random.Shared.Next(Names.Length)],
                Id = index,
                Soyadi = "Test Surname",
                TelefonNumarasi = "123132",
                Email = "Test Email"
            }).ToArray();

            return a;
        }

        [HttpPost("CreateOrUpdateCustomer")]
        [AllowAnonymous]
        public ApiResult CreateOrUpdateCustomer([FromBody] CustomerCreateVM dataVM)
        {
            if (!ModelState.IsValid)
                return new ApiResult { Result = false, Message = "Form'da doldurulmayan alanlar mevcut,lütfen doldurun." };
            Customer data = null;
            if (dataVM.Id > 0)
                data = _unitOfWork.Repository<Customer>().GetById(dataVM.Id);
            else
                data = new Customer()
                {
                    Adi = dataVM.Adi,
                    Soyadi = dataVM.Soyadi,
                    Email = dataVM.Email,
                    TelefonNumarasi = dataVM.TelefonNumarasi,
                };


            _unitOfWork.Repository<Customer>().InsertOrUpdate(data);
            _unitOfWork.SaveChanges();
            return new ApiResult { Result = true };
        }

        [HttpPost("GetCustomerGrid")]
        [AllowAnonymous]
        public ApiResult<GridResultModel<CustomerGridVM>> GetCustomerGrid()
        {

            var query = _unitOfWork.Repository<Customer>()
            .Select(x => new CustomerGridVM
            {
                Id = x.Id,
                Adi = x.Adi,
                Soyadi = x.Soyadi,
                Email = x.Email,
                TelefonNumarasi = x.TelefonNumarasi,
            });

            var rest = query.ToDataListRequest(Request.ToRequestFilter());

            return new ApiResult<GridResultModel<CustomerGridVM>> { Data = rest, Result = true };
        }

    }
}

