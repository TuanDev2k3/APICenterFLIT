using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebTinTucFLIC.Entities;
using WebTinTucFLIC.Helper;
using WebTinTucFLIC.Models;

namespace WebTinTucFLIC.Controllers
{
	public class HomeController : Controller
	{ 
		private readonly IConfiguration _configuration;
        private readonly APICaller<NewsDTO> _apiCall;
        Response res = new Response();

        public HomeController(IConfiguration configuration)
		{
			_configuration = configuration;
            _apiCall = new APICaller<NewsDTO>(_configuration);
        }

        [Authorize(Roles = "ADM, UP, COD")]
        public async Task<IActionResult> Dashboad()
		{
            // Biểu đồ bài viết
            res = await _apiCall.GetListAsync($"News/GetArticlesCountByMonth?year=2024");
            if (res.Result == 1)
            {
                var danhsach = res.Data!.ToObject<List<dynamic>>();
                List<int> listCount = new List<int>();
                List<string> listMonth = new List<string>();
				int newcount = 0; // đém tổng số lượng bài viêt
                foreach (var i in danhsach)
				{
					int item = Convert.ToInt32(i.count);
                    string month = Convert.ToString($"Tháng {i.month}");
                    listCount.Add(item);
                    listMonth.Add(month);
					newcount += item;
                }
				ViewBag.ListCount = listCount.ToArray();
				ViewBag.ListMonth = listMonth.ToArray();
				ViewBag.NewCount = newcount;
            }

            // QL Top 2 bình luận
            res = await _apiCall.GetListAsync($"Comment/Pagedlist?page=1&pageSize=3");
            if (res.Result == 1)
            {
                var dscomment = res.Data!.data.ToObject<List<Comment>>();
                ViewBag.DSComment = dscomment;
            }

            // Top 2 bai viet nong moi nhat
            res = await _apiCall.GetListAsync($"News/Top2HotBetter");
            if (res.Result == 1)
            {
                var dsnewhot = res.Data!.ToObject<List<NewsDTO>>();
                ViewBag.DSNewHots = dsnewhot;
            }

            // Bai viet moi nhat
            res = await _apiCall.GetListAsync($"News/Pagedlist?page=1&pageSize=6");
            if (res.Result == 1)
            {
                var dslatestnews = res.Data!.data.ToObject<List<NewsDTO>>();
                ViewBag.DSLastNews = dslatestnews;
            }
            return View();
		}


		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
