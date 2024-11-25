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
	public class AccountTypeController : ControllerBase
	{
		private readonly IAccountTypeService _service;
		Response res = new Response();
		public AccountTypeController(IAccountTypeService service)
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
		public async Task<Response> Search(string search)
		{
			try
			{
				Expression<Func<AccountType, bool>> filter;
				filter = x => x.Status == 1 && (x.Code.Contains(search) || x.Description!.Contains(search) || x.Name.Contains(search));
				res = await _service.Search(filter);
			}
			catch (Exception ex)
			{
				res.Status = 0;
				res.Message = ex.Message;
			}
			return res;
		}
		[HttpPost]
		public async Task<Response> Create(AccountTypeDTO model, int userId)
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
		public async Task<Response> Update(AccountTypeDTO model, int id, int userId)
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
		public async Task<Response> Delete(int id, int userId)
		{
			try
			{
				res = await _service.Delete(id, userId);
			}
			catch (Exception ex)
			{
				res.Status = 0;
				res.Message = ex.Message;
			}
			return res;
		}

        [HttpGet]
        public async Task<Response> SelectDropDown()
        {
            try
            {
                res = await _service.SelectDropDown();
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
