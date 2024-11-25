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
	public class NewsController : ControllerBase
	{
		private readonly INewsService _service;
		Response res = new Response();
		public NewsController(INewsService service)
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
        public async Task<Response> Top2HotBetter()
        {
            try
            {
                res = await _service.Top2HotBetter();
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
				Expression<Func<News, bool>> filter;
				filter = x => x.Status == 1 && (x.Title.Contains(search) || x.Description!.Contains(search) || x.Detail.Contains(search));
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
		public async Task<ActionResult<Response>> Pagedlist(int page, int pageSize, int? newstypeId, int? userId, bool? review)
		{
			try
			{
				res = await _service.PaginationAsync(page, pageSize, newstypeId, userId, review);
			}
			catch (Exception ex)
			{
				res.Status = 0;
				res.Message = $"Exception: Xảy ra lỗi khi đọc dữ liệu {ex.Message} InnerException {ex.InnerException?.Message}";
			}
			return res;
		}

        [HttpGet]
        public async Task<ActionResult<Response>> PageListUI(int page, int pageSize, int? newtypeId)
        {
            try
            {
                res = await _service.PageList(page, pageSize, newtypeId);
            }
            catch (Exception ex)
            {
                res.Status = 0;
                res.Message = $"Exception: Xảy ra lỗi khi đọc dữ liệu {ex.Message} InnerException {ex.InnerException?.Message}";
            }
            return res;
        }

        //[HttpGet]
        //public async Task<ActionResult<Response>> NhanBan()
        //{
        //    try
        //    {
        //        res = await _service.NhanBan();
        //    }
        //    catch (Exception ex)
        //    {
        //        res.Status = 0;
        //        res.Message = $"Exception: Xảy ra lỗi khi đọc dữ liệu {ex.Message} InnerException {ex.InnerException?.Message}";
        //    }
        //    return res;
        //}

        [HttpPost]
		public async Task<Response> Create(NewsDTO model, int userId)
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
		public async Task<Response> Update(NewsDTO model, int id, int userId)
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

        [HttpDelete]
        public async Task<Response> ChapNhanDuyetBai(int id, int userId, int ishot, int homepage)
        {
            try
            {
                res = await _service.ChapNhanDuyetBai(id, userId, ishot, homepage);
            }
            catch (Exception ex)
            {
                res.Status = 0;
                res.Message = ex.Message;
            }
            return res;
        }

        [HttpDelete]
        public async Task<Response> TuChoiDuyetBai(int id, int userId)
        {
            try
            {
                res = await _service.TuChoiDuyetBai(id, userId);
            }
            catch (Exception ex)
            {
                res.Status = 0;
                res.Message = ex.Message;
            }
            return res;
        }

		[HttpGet]
        public async Task<Response> GetArticlesCountByMonth(int year)
        {
            try
            {
                res = await _service.GetArticlesCountByMonth(year);
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
