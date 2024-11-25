using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebTinTucFLIC.Entities;
using WebTinTucFLIC.Helper;
using WebTinTucFLIC.Models;

namespace WebTinTucFLIC.Controllers
{
    public class NewsController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly APICaller<NewsDTO> _apiCall;
        private readonly APICaller<Comment> _apiCallCmt;
        private readonly APICaller<NewsTypeDTO> _apiCallNewsType;
        private readonly ImageController _imageController;
        private readonly AccessLogController _accessLogController;
        Response res = new Response();
        public NewsController(IConfiguration configuration, ImageController imageController, AccessLogController accessLogController)
        {
            _configuration = configuration;
            _apiCall = new APICaller<NewsDTO>(_configuration);
            _apiCallNewsType = new APICaller<NewsTypeDTO>(_configuration);
            _apiCallCmt = new APICaller<Comment>(_configuration);
            _imageController = imageController;
            _accessLogController = accessLogController;
        }
        public async Task<IActionResult> Blog(int? id, int? page = 1, int? pageSize = 10)
        {
            var viewModel = new NewsViewModel();

            // Nếu có id, lấy thông tin chi tiết bài viết theo id
            if (id.HasValue)
            {
                res = await _apiCall.GetListAsync($"News/EditById?id={id.Value}");
                if (res != null && res.Result == 1 && res.Data != null)
                {
                    viewModel.SelectedNews = res.Data.ToObject<NewsDTO>();

                    int? newsTypeId = viewModel.SelectedNews?.NewTypeId;

                    if (newsTypeId.HasValue)
                    {
                        res = await _apiCall.GetListAsync($"/News/PageListUI?page={page}&pageSize={pageSize}&newsTypeId={newsTypeId.Value}");
                        if (res != null && res.Result == 1 && res.Data != null)
                        {
                            viewModel.NewsList = res.Data.data?.ToObject<List<NewsDTO>>();
                        }
                    }
                }
            }
            else
            {
                // Nếu không có id, lấy danh sách bài viết chung
                res = await _apiCall.GetListAsync($"/News/PageListUI?page={page}&pageSize={pageSize}");
                if (res != null && res.Result == 1 && res.Data != null)
                {
                    viewModel.NewsList = res.Data.data?.ToObject<List<NewsDTO>>();
                }
            }

            // Kiểm tra và thiết lập ViewBag cho phân trang
            if (res?.Data != null)
            {
                ViewBag.PageTotal = Convert.ToInt32(res.Data.count);
            }

            res = await _apiCallCmt.GetListAsync($"Comment/ListCommentsByNewId?neweId={id}");
            if (res.Result == 1 && res.Data != null)
            {
                var comments = res.Data!.ToObject<List<Comment>>();
                ViewBag.Comments = comments;
            }
            ViewBag.CurrentPage = page;
            ViewBag.NewVM = viewModel;

            return View();
        }
        public async Task<IActionResult> Detail(int? page = 1, int? pageSize = 10, int? newtypeId = 0)
        {
            //Them tra ve danh sach News 
            string url = $"/News/PageListUI?page={page}&pageSize={pageSize}";
            if (newtypeId != 0)
            {
                url += $"&newtypeId={newtypeId}";
            }

            res = await _apiCall.GetListAsync(url);

            if (res.Result == 1)
            {
                var danhsach = res.Data!.data.ToObject<List<NewsDTO>>();
                ViewBag.PageTotal = Convert.ToInt32(res.Data!.count);
                ViewBag.CurrentPage = page;
                ViewBag.SelectedNewsTypeId = newtypeId;
                return View(danhsach);
            }
            else
            {
                return View();
            }
        }
        public async Task<IActionResult> Index()
        {
            res = await _apiCallNewsType.GetListAsync($"NewsType/GetList");
            if (res.Result == 1)
            {
                List<NewsTypeDTO> danhsach = res.Data!.ToObject<List<NewsTypeDTO>>();
                danhsach = danhsach.Where(x => x.HomePage == 1).ToList();
                ViewBag.Counts = danhsach;
            }
            else
            {

            }
            //Them tra ve danh sach News 
            res = await _apiCall.GetListAsync($"News/Top2HotBetter");
            if (res.Result == 1)
            {
                var danhsachs = res.Data!.ToObject<List<NewsDTO>>();
                ViewBag.news = danhsachs;
            }
            return View();
        }

        // Phan Quan ly 

        public async Task<List<NewsType>> SelectDropDown()
        {
            res = await _apiCall.GetListAsync($"NewsType/SelectDropDown");
            if (res.Result == 1)
            {
                var listnewstype = res.Data!.ToObject<List<NewsType>>();
                return listnewstype;
            }
            return null!;
        }

        [Authorize(Roles = "ADM, UP, COD")]
        public async Task<IActionResult> P_List(int? page = 1, int? pageSize = 10, int? newtypeId = 0)
        {
            string url = $"/News/Pagedlist?page={page}&pageSize={pageSize}";
            var role = User.FindFirst(ClaimTypes.Role)?.Value!;
            var userId = int.Parse(User.FindFirst("UserId")?.Value!);
            if (newtypeId != 0)
            {
                url += $"&newstypeId={newtypeId}";
            }
            if (role == "UP")
            {
                url += $"&userId={userId}";
            }
            if (role == "COD")
            {
                url += $"&review=true";
            }
            res = await _apiCall.GetListAsync(url);

            if (res.Result == 1)
            {
                var danhsach = res.Data!.data.ToObject<List<NewsDTO>>();
                ViewBag.PageTotal = Convert.ToInt32(res.Data!.count);
                ViewBag.CurrentPage = page;
                ViewBag.DsNewsType = await SelectDropDown();
                ViewBag.SelectedNewsTypeId = newtypeId;
                return View(danhsach);
            }
            else
            {
                return View();
            }
        }

        [Authorize(Roles = "ADM, COD")]
        public async Task<IActionResult> ChapNhanDuyetBai(int id, int ishot = 0, int homepage = 0)
        {
            try
            {
                var userId = int.Parse(User.FindFirst("UserId")?.Value!);
                var name = User.FindFirst(ClaimTypes.Name)?.Value;
                res = await _apiCall.DeleteAsync($"News/ChapNhanDuyetBai?id={id}&userId={userId}&ishot={ishot}&homepage={homepage}");
                if (res.Result == 1)
                {
                    var access = new AccessLog
                    {
                        AccountId = userId,
                        Description = $"{name} vừa chấp nhận duyệt bài viết số {id}",
                        DeviceName = _accessLogController.GetDeviceName(HttpContext.Request.Headers["User-Agent"].ToString()),
                        BrowersName = _accessLogController.GetBrowserName(HttpContext.Request.Headers["User-Agent"].ToString()),
                        Timer = DateTime.Now,
                        IpAddress = HttpContext.Connection.RemoteIpAddress!.ToString()
                    };
                    await _accessLogController.CreateAccessLog(access);
                    return Redirect("/News/P_List");
                }
            }
            catch (Exception ex)
            {
                res.Message = $"Có lỗi khi tìm kiếm dữ liệu {ex.Message}. InnerException {ex.InnerException?.Message}";
            }
            return View("/Review");
        }

        [Authorize(Roles = "ADM, COD")]
        public async Task<IActionResult> TuChoiDuyetBai(int id)
        {
            try
            {
                var userId = int.Parse(User.FindFirst("UserId")?.Value!);
                var name = User.FindFirst(ClaimTypes.Name)?.Value;
                res = await _apiCall.DeleteAsync($"News/TuChoiDuyetBai?id={id}&userId={userId}");
                if (res.Result == 1)
                {
                    var access = new AccessLog
                    {
                        AccountId = userId,
                        Description = $"{name} vừa từ chối duyệt bài viết số {id}",
                        DeviceName = _accessLogController.GetDeviceName(HttpContext.Request.Headers["User-Agent"].ToString()),
                        BrowersName = _accessLogController.GetBrowserName(HttpContext.Request.Headers["User-Agent"].ToString()),
                        Timer = DateTime.Now,
                        IpAddress = HttpContext.Connection.RemoteIpAddress!.ToString()
                    };
                    await _accessLogController.CreateAccessLog(access);
                    return Redirect("/News/P_List");
                }
            }
            catch (Exception ex)
            {
                res.Message = $"Có lỗi khi tìm kiếm dữ liệu {ex.Message}. InnerException {ex.InnerException?.Message}";
            }
            return View("/Review");
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
        public async Task<IActionResult> Create(NewsDTO model, IFormFile imageFile)
        {
            try
            {
                var userId = int.Parse(User.FindFirst("UserId")?.Value!);
                var name = User.FindFirst(ClaimTypes.Name)?.Value;
                var role = User.FindFirst(ClaimTypes.Role)?.Value!;

                if (imageFile != null && imageFile.Length > 0)
                {
                    model.ImageUrl = myUltilities.UploadImage(imageFile, "news");
                    model.ImageId = await _imageController.CreateImage(model.ImageUrl, userId);
                }
                else
                {
                    model.ImageId = 1;
                    model.ImageUrl = "Default.jpg";
                }

                res = await _apiCall.CreateAsync($"News/Create?userId={userId}", model);
                if (res.Result == 1)
                {
                    if (role == "ADM")
                    {
                        var newsId = Convert.ToInt32(res.Data!);
                        ChapNhanDuyetBai(newsId);
                    }
                    var access = new AccessLog
                    {
                        AccountId = userId,
                        Description = $"{name} vừa tạo một bài viết mới",
                        DeviceName = _accessLogController.GetDeviceName(HttpContext.Request.Headers["User-Agent"].ToString()),
                        BrowersName = _accessLogController.GetBrowserName(HttpContext.Request.Headers["User-Agent"].ToString()),
                        Timer = DateTime.Now,
                        IpAddress = HttpContext.Connection.RemoteIpAddress!.ToString()
                    };
                    await _accessLogController.CreateAccessLog(access);
                    return Redirect("/News/P_List");
                }
            }
            catch (Exception ex)
            {
                res.Message = $"Có lỗi khi tìm kiếm dữ liệu {ex.Message}. InnerException {ex.InnerException?.Message}";
            }
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "ADM, UP")]
        public async Task<IActionResult> Update(int id, string title)
        {
            res = await _apiCall.GetListAsync($"News/EditById?id={id}");
            if (res.Result == 1)
            {
                var news = res.Data!.ToObject<NewsDTO>();
                ViewBag.DsNewsType = await SelectDropDown();
                return View(news);
            }

            return View("P_List");
        }

        [HttpPost] 
        [Authorize(Roles = "ADM, UP")]
        public async Task<IActionResult> Update(NewsDTO model, int id, IFormFile imageFile)
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


                res = await _apiCall.UpdateAsync($"News/Update?id={id}&userId={userId}", model);
                if (res.Result == 1)
                {
                    var access = new AccessLog
                    {
                        AccountId = userId,
                        Description = $"{name} vừa cập nhật lại thông tin bài viết số {id}",
                        DeviceName = _accessLogController.GetDeviceName(HttpContext.Request.Headers["User-Agent"].ToString()),
                        BrowersName = _accessLogController.GetBrowserName(HttpContext.Request.Headers["User-Agent"].ToString()),
                        Timer = DateTime.Now,
                        IpAddress = HttpContext.Connection.RemoteIpAddress!.ToString()
                    };
                    await _accessLogController.CreateAccessLog(access);
                    return Redirect("/News/P_List");
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
                res = await _apiCall.DeleteAsync($"News/Delete?id={id}&userId={userId}");
                if (res.Result == 1)
                {
                    var access = new AccessLog
                    {
                        AccountId = userId,
                        Description = $"{name} vừa khóa xóa bài viết số {id}",
                        DeviceName = _accessLogController.GetDeviceName(HttpContext.Request.Headers["User-Agent"].ToString()),
                        BrowersName = _accessLogController.GetBrowserName(HttpContext.Request.Headers["User-Agent"].ToString()),
                        Timer = DateTime.Now,
                        IpAddress = HttpContext.Connection.RemoteIpAddress!.ToString()
                    };
                    await _accessLogController.CreateAccessLog(access);
                    return Redirect("/News/P_List");
                }
            }
            catch (Exception ex)
            {
                res.Message = $"Có lỗi khi tìm kiếm dữ liệu {ex.Message}. InnerException {ex.InnerException?.Message}";
            }
            return View("P_List");
        }

        [Authorize]
        public async Task<IActionResult> Review(int id, string title)
        {
            res = await _apiCall.GetListAsync($"News/EditById?id={id}");
            if (res.Result == 1)
            {
                var news = res.Data!.ToObject<NewsDTO>();
                return View(news);
            }
            return View("P_List");
        }

    }
}
