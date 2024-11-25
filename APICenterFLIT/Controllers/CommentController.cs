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
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _service;
        Response res = new Response();
        public CommentController(ICommentService service)
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
                Expression<Func<Comment, bool>> filter;
                filter = x => x.Status == 1 && (x.Detail.Contains(search));
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

        [HttpGet]
        public async Task<Response> ListCommentsByNewId(int neweId)
        {
            try
            {
                res = await _service.ListCommentsByNewId(neweId); 
            }
            catch (Exception ex)
            {
                res.Status = 0;
                res.Message = ex.Message;
            }
            return res;
        }

        [HttpPost]
        public async Task<Response> Create(CommentDTO model, int userId)
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
        public async Task<Response> Update(CommentDTO model, int id, int userId)
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
    }
}
