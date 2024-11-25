using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebTinTucFLIC.Entities;
using WebTinTucFLIC.Helper;
using WebTinTucFLIC.Models;

namespace WebTinTucFLIC.Controllers
{
    public class NewsTypeController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly APICaller<NewsTypeDTO> _apiCall;
        private readonly ImageController _imageController;
        private readonly AccessLogController _accessLogController;
        Response res = new Response();
        public NewsTypeController(IConfiguration configuration, ImageController imageController, AccessLogController accessLogController)
        {
            _configuration = configuration;
            _apiCall = new APICaller<NewsTypeDTO>(_configuration);
            _imageController = imageController;
            _accessLogController = accessLogController;
        }
        [Authorize]
        public async Task<IActionResult> P_List(int? page = 1, int? pageSize = 10)
        {
            res = await _apiCall.GetListAsync($"/NewsType/Pagedlist?page={page}&pageSize={pageSize}");
            if (res.Result == 1)
            {
                var danhsach = res.Data!.data.ToObject<List<NewsTypeDTO>>();
                ViewBag.PageTotal = Convert.ToInt32(res.Data!.count);
                ViewBag.CurrentPage = page;
                return View(danhsach);
            }
            else
            {
                return View();
            }
        }

        public async Task<List<NewsType>> SelectDropDown()
        {
            res = await _apiCall.GetListAsync($"NewsType/SelectDropDown");
            if (res.Result == 1)
            {
                var danhsach = res.Data!.ToObject<List<NewsType>>();
                return danhsach;
            }
            return null!;
        }

        [HttpGet]
        [Authorize(Roles = "ADM, UP")]
        public async Task<IActionResult> Create()
        {
            ViewBag.DsNewsType = await SelectDropDown();
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "ADM, UP")]
        public async Task<IActionResult> Create(NewsTypeDTO model, IFormFile imageFile)
        {
            try
            {
                var userId = int.Parse(User.FindFirst("UserId")?.Value!);
                var name = User.FindFirst(ClaimTypes.Name)?.Value;
                if (imageFile != null && imageFile.Length > 0)
                {
                    model.ImageUrl = myUltilities.UploadImage(imageFile, "newstype");
                    model.ImageId = await _imageController.CreateImage(model.ImageUrl, userId);
                }
                else
                {
                    model.ImageId = 1;
                    model.ImageUrl = "Default.jpg";
                }

                res = await _apiCall.CreateAsync($"/NewsType/Create?userId={userId}", model);
                if (res.Result == 1)
                {
                    var access = new AccessLog
                    {
                        AccountId = userId,
                        Description = $"{name} vừa tạo thể loại bài viết mới với Id = {res.Data}",
                        DeviceName = _accessLogController.GetDeviceName(HttpContext.Request.Headers["User-Agent"].ToString()),
                        BrowersName = _accessLogController.GetBrowserName(HttpContext.Request.Headers["User-Agent"].ToString()),
                        Timer = DateTime.Now,
                        IpAddress = HttpContext.Connection.RemoteIpAddress!.ToString()
                    };
                    await _accessLogController.CreateAccessLog(access);
                    return Redirect("/NewsType/P_List");
                }
            }
            catch (Exception ex)
            {
                res.Message = $"Có lỗi khi tìm kiếm dữ liệu {ex.Message}. InnerException {ex.InnerException?.Message}";
            }
            return View("Create");
        }

        [HttpGet]
        [Authorize(Roles = "ADM, UP")]
        public async Task<IActionResult> Update(int id)
        {
            res = await _apiCall.GetListAsync($"/NewsType/EditById?id={id}");
            if (res.Result == 1)
            {
                var news = res.Data!.ToObject<NewsTypeDTO>();
                ViewBag.DsNewsType = await SelectDropDown();
                return View(news);
            }

            return View();
        }

        [HttpPost]
        [Authorize(Roles = "ADM, UP")]
        public async Task<IActionResult> Update(NewsTypeDTO model, int id, IFormFile imageFile)
        {
            try
            {
                var userId = int.Parse(User.FindFirst("UserId")?.Value!);
                var name = User.FindFirst(ClaimTypes.Name)?.Value;
                if (imageFile != null && imageFile.Length > 0)
                {
                    model.ImageUrl = myUltilities.UploadImage(imageFile, "news");
                    model.ImageId = await _imageController.CreateImage(model.ImageUrl, userId);
                }

                res = await _apiCall.UpdateAsync($"NewsType/Update?id={id}&userId={userId}", model);
                if (res.Result == 1)
                {
                    var access = new AccessLog
                    {
                        AccountId = userId,
                        Description = $"{name} vừa cập nhật lại thông tin thể loại bài viết với Id = {id}",
                        DeviceName = _accessLogController.GetDeviceName(HttpContext.Request.Headers["User-Agent"].ToString()),
                        BrowersName = _accessLogController.GetBrowserName(HttpContext.Request.Headers["User-Agent"].ToString()),
                        Timer = DateTime.Now,
                        IpAddress = HttpContext.Connection.RemoteIpAddress!.ToString()
                    };
                    await _accessLogController.CreateAccessLog(access);
                    return Redirect("/NewsType/P_List");
                }
            }
            catch (Exception ex)
            {
                res.Message = $"Có lỗi khi tìm kiếm dữ liệu {ex.Message}. InnerException {ex.InnerException?.Message}";
            }
            return View("Update");
        }

        [Authorize(Roles = "ADM, UP")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var userId = int.Parse(User.FindFirst("UserId")?.Value!);
                var name = User.FindFirst(ClaimTypes.Name)?.Value;
                res = await _apiCall.DeleteAsync($"/NewsType/Delete?id={id}&userId={userId}");
                if (res.Result == 1)
                {
                    var access = new AccessLog
                    {
                        AccountId = userId,
                        Description = $"{name} vừa xóa thể loại bài viết số {id}",
                        DeviceName = _accessLogController.GetDeviceName(HttpContext.Request.Headers["User-Agent"].ToString()),
                        BrowersName = _accessLogController.GetBrowserName(HttpContext.Request.Headers["User-Agent"].ToString()),
                        Timer = DateTime.Now,
                        IpAddress = HttpContext.Connection.RemoteIpAddress!.ToString()
                    };
                    await _accessLogController.CreateAccessLog(access);
                    return Redirect("/NewsType/P_List");
                }
            }
            catch (Exception ex)
            {
                res.Message = $"Có lỗi khi tìm kiếm dữ liệu {ex.Message}. InnerException {ex.InnerException?.Message}";
            }
            return Redirect("/NewsType/P_List");
        }
    }
}
