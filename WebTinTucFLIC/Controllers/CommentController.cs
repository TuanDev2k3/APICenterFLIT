using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebTinTucFLIC.Helper;
using WebTinTucFLIC.Models;

namespace WebTinTucFLIC.Controllers
{
    
    public class CommentController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly APICaller<Comment> _apiCall;
        private readonly ImageController _imageController;
        private readonly AccessLogController _accessLogController;
        Response res = new Response();
        public CommentController(IConfiguration configuration, ImageController imageController, AccessLogController accessLogController)
        {
            _configuration = configuration;
            _apiCall = new APICaller<Comment>(_configuration);
            _imageController = imageController;
            _accessLogController = accessLogController;
        }
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "ADM, UP")]
        public async Task<IActionResult> P_List(int? page = 1, int? pageSize = 10)
        {
            string url = $"/News/Pagedlist?page={page}&pageSize={pageSize}";
            var userId = int.Parse(User.FindFirst("UserId")?.Value!);
            var rolename = User.FindFirst(ClaimTypes.Role)?.Value!;
            if (rolename == "UP")
            {
                url += $"&userId={userId}";
            }
            res = await _apiCall.GetListAsync(url);

            if (res.Result == 1)
            {
                var danhsach = res.Data!.data.ToObject<List<NewsDTO>>();
                ViewBag.PageTotal = Convert.ToInt32(res.Data!.count);
                ViewBag.CurrentPage = page;
                return View(danhsach);
            }
            else
            {
                return View();
            }
        }

        [Authorize(Roles = "ADM, UP")]
        public async Task<IActionResult> Get(int id, string? title)
        {
            if (id != 0)
            {
                res = await _apiCall.GetListAsync($"News/EditById?id={id}");
                if (res.Result == 1 && res.Data != null)
                {
                    var news = res.Data!.ToObject<NewsDTO>();
                    ViewBag.News = news;
                }

                res = await _apiCall.GetListAsync($"Comment/ListCommentsByNewId?neweId={id}");
                if (res.Result == 1 && res.Data != null)
                {
                    var comments = res.Data!.ToObject<List<Comment>>();
                    ViewBag.Comments = comments;
                }
            }
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "ADM, UP")]
        public async Task<IActionResult> Delete(int id) 
        {
            try
            {
                var userId = int.Parse(User.FindFirst("UserId")?.Value!);
                var name = User.FindFirst(ClaimTypes.Name)?.Value;
                res = await _apiCall.DeleteAsync($"Comment/Delete?id={id}&userId={userId}");
                if (res.Result == 1)
                {
                    var access = new AccessLog
                    {
                        AccountId = userId,
                        Description = $"{name} vừa xóa một bình luận ({id})",
                        DeviceName = _accessLogController.GetDeviceName(HttpContext.Request.Headers["User-Agent"].ToString()),
                        BrowersName = _accessLogController.GetBrowserName(HttpContext.Request.Headers["User-Agent"].ToString()),
                        Timer = DateTime.Now,
                        IpAddress = HttpContext.Connection.RemoteIpAddress!.ToString()
                    };
                    await _accessLogController.CreateAccessLog(access);
                    return Json(new { success = true });
                }
                return Json(new { success = false, message = "Không tìm thấy bình luận." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(Comment model)
        {
            try
            {
                var userId = int.Parse(User.FindFirst("UserId")?.Value!);
                var name = User.FindFirst(ClaimTypes.Name)?.Value;

                var res = await _apiCall.CreateAsync($"Comment/Create?userId={userId}", model);
                if (res.Result == 1)
                {
                    var access = new AccessLog
                    {
                        AccountId = userId,
                        Description = $"{name} vừa bình luận một bài viết ({model.NewId})",
                        DeviceName = _accessLogController.GetDeviceName(HttpContext.Request.Headers["User-Agent"].ToString()),
                        BrowersName = _accessLogController.GetBrowserName(HttpContext.Request.Headers["User-Agent"].ToString()),
                        Timer = DateTime.Now,
                        IpAddress = HttpContext.Connection.RemoteIpAddress!.ToString()
                    };
                    await _accessLogController.CreateAccessLog(access);

                    return Redirect(Request.Headers["Referer"].ToString());
                }

                TempData["SuccessMessage"] = "Không thể bình luận";
                return Redirect(Request.Headers["Referer"].ToString());
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return Redirect(Request.Headers["Referer"].ToString());
            }
        }


    }
}
