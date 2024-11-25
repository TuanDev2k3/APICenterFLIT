using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebTinTucFLIC.Helper;
using WebTinTucFLIC.Models;

namespace WebTinTucFLIC.Controllers
{
    [Authorize(Roles = "ADM")]
    public class AccessLogController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly APICaller<AccessLog> _apiCall;
        private readonly ImageController _imageController;
        Response res = new Response();
        public AccessLogController(IConfiguration configuration, ImageController imageController)
        {
            _configuration = configuration;
            _apiCall = new APICaller<AccessLog>(_configuration);
            _imageController = imageController;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> P_List(int? page = 1, int? pageSize = 10)
        {
            string url = $"/AccessLog/Pagedlist?page={page}&pageSize={pageSize}";
            res = await _apiCall.GetListAsync(url);

            if (res.Result == 1)
            {
                var danhsach = res.Data!.data.ToObject<List<AccessLog>>();
                ViewBag.PageTotal = Convert.ToInt32(res.Data!.count);
                ViewBag.CurrentPage = page;
                return View(danhsach);
            }
            else
            {
                return View();
            }
        }

        public string GetBrowserName(string userAgent)
        {
            if (userAgent.Contains("Chrome") && !userAgent.Contains("Edg"))
                return "Chrome";
            if (userAgent.Contains("Edg"))
                return "Edge";
            if (userAgent.Contains("Firefox"))
                return "Firefox";
            if (userAgent.Contains("Safari") && !userAgent.Contains("Chrome"))
                return "Safari";
            if (userAgent.Contains("OPR") || userAgent.Contains("Opera"))
                return "Opera";
            if (userAgent.Contains("CocCoc"))
                return "CocCoc";

            return "Unknown";
        }

        public string GetDeviceName(string userAgent)
        {
            if (userAgent.Contains("iPhone") || userAgent.Contains("iPad"))
                return "Apple Device";
            if (userAgent.Contains("Android"))
                return "Android Device";
            if (userAgent.Contains("Windows"))
                return "Windows PC";
            if (userAgent.Contains("Macintosh"))
                return "Mac Device";
            if (userAgent.Contains("Linux"))
                return "Linux Device";

            return "Unknown Device";
        }

        public async Task CreateAccessLog(AccessLog acc)
        {
            res = await _apiCall.CreateAsync("AccessLog/Create", acc);
        }
    }
}
