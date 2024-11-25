using System.Linq.Expressions;
using APICenterFlit.Entities;
using APICenterFlit.Helper;
using APICenterFlit.Models;
using APICenterFlit.Repositories.Portal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APICenterFlit.Controllers
{
	[Route("[controller]/[action]")]
	[ApiController]
	public class AccountController : ControllerBase
	{
		private readonly IAccountService _service;
		Response res = new Response();
		public AccountController(IAccountService service)
		{
			_service = service;
		}

		[HttpGet]
		public async Task<Response> GetList()
		{
			try
			{
				res = await _service.GetListAsync();
			}
			catch (Exception ex)
			{
				res.Status = 0;
				res.Message = ex.Message;
			}
			return res;
		}

		[HttpGet]
		public async Task<Response> EditById(int id)
		{
			try
			{
				res = await _service.EditById(id);
			}
			catch (Exception ex)
			{
				res.Status = 0;
				res.Message = ex.Message;
			}
			return res;
		}

        [HttpGet]
        public async Task<Response> IsLockedGoogle(string email)
        {
            try
            {
                res = await _service.IsLockedGoogle(email);
            }
            catch (Exception ex)
            {
                res.Status = 0;
                res.Message = ex.Message;
            }
            return res;
        }
        [HttpGet]
		public async Task<Response> Search(string search)
		{
			try
			{
				Expression<Func<Account, bool>> filter;
				filter = x => x.Status == 1 && (x.UserName.Contains(search) || x.Fullname!.Contains(search) || x.Phone!.Contains(search));
				res = await _service.Search(filter);
			}
			catch (Exception ex)
			{
				res.Status = 0;
				res.Message = ex.Message;
			}
			return res;
		}

        [HttpGet]
        public async Task<ActionResult<Response>> Pagedlist(int page, int pageSize, int? accountTypeId, int? status = 0)
        {
            try
            {
                res = await _service.PaginationAsync(page, pageSize, accountTypeId, status);
            }
            catch (Exception ex)
            {
                res.Status = 0;
                res.Message = $"Exception: Xảy ra lỗi khi đọc dữ liệu {ex.Message} InnerException {ex.InnerException?.Message}";
            }
            return res;
        }

        [HttpPost]
		public async Task<ActionResult<Response>> Login([FromBody] LoginModal modal)
		{
			try
			{
				res = await _service.Login(modal);
			}
			catch (Exception ex)
			{
				res.Status = 0;
				res.Message = $"Exception: Xảy ra lỗi khi đọc dữ liệu {ex.Message} InnerException {ex.InnerException?.Message}";
			}
			return res;
		}
		[HttpPost]
		public async Task<Response> Create([FromBody] AccountDTO model, int userId)
		{
			try
			{
				res = await _service.Create(model, userId);
			}
			catch (Exception ex)
			{
				res.Status = 0;
				res.Message = ex.Message;
			}
			return res;
		}

		[HttpPut]
		public async Task<Response> Update([FromBody] AccountDTO model, int id, int userId)
		{
			try
			{
				res = await _service.Update(model, id, userId);
			}
			catch (Exception ex)
			{
				res.Status = 0;
				res.Message = ex.Message;
			}
			return res;
		}

		[HttpDelete]
		public async Task<Response> Lock( int id, int userId)
		{
			try
			{
				res = await _service.Lock(id, userId);
			}
			catch (Exception ex)
			{
				res.Status = 0;
				res.Message = ex.Message;
			}
			return res;
		}

		[HttpPut]
		public async Task<Response> ChangePassword(ChangePasswordViewModel modal, int id)
		{
			try
			{
				res = await _service.ChangePassword(modal, id);
			}
			catch (Exception ex)
			{
				res.Status = 0;
				res.Message = ex.Message;
			}
			return res;
		}
	}
}
