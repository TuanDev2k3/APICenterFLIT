using System.Linq.Expressions;
using APICenterFlit.Entities;
using APICenterFlit.Helper;
using APICenterFlit.Models;
using APICenterFlit.Repositories.Portal;
using APICenterFlit.Repositories.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APICenterFlit.Controllers
{
	[Route("[controller]/[action]")]
	[ApiController]
	public class AccessLogController : ControllerBase
	{
		private readonly IAccessLogService _service;
		Response res = new Response();
		public AccessLogController(IAccessLogService service)
		{
			_service = service;
		}

		[HttpGet]
		public async Task<ActionResult<Response>> Pagedlist(int page, int pageSize)
		{
			try
			{
				res = await _service.PaginationAsync(page, pageSize);
			}
			catch (Exception ex)
			{
				res.Status = 0;
				res.Message = $"Exception: Xảy ra lỗi khi đọc dữ liệu {ex.Message} InnerException {ex.InnerException?.Message}";
			}
			return res;
		}
		[HttpPost]
		public async Task<Response> Create(AccessLogDTO model)
		{
			try
			{
				res = await _service.Create(model);
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
