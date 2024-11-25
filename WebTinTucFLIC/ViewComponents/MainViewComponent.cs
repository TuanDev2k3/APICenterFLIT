using Microsoft.AspNetCore.Mvc;
using WebTinTucFLIC.Helper;
using WebTinTucFLIC.Models;

namespace WebTinTucFLIC.ViewComponents
{
    public class MainViewComponent : ViewComponent
    {
        private readonly IConfiguration _configuration;
        private readonly APICaller<NewsTypeDTO> _apiCall;
        private readonly APICaller<NewsDTO> _apiCallNews;
        Response res = new Response();
        public MainViewComponent(IConfiguration configuration)
        {
            _configuration = configuration;
            _apiCall = new APICaller<NewsTypeDTO>(_configuration);
            _apiCallNews = new APICaller<NewsDTO>(_configuration);
        }

        // Thay đổi tên phương thức thành InvokeAsync
        public async Task<IViewComponentResult> InvokeAsync(int newsTypeId)
        {
            // Gọi phương thức SelectDropDown và lấy danh sách NewsType
            res = await _apiCall.GetListAsync($"/NewsType/EditById?id={newsTypeId}");
            if (res.Result == 1)
            {
                var newsType = res.Data!.ToObject<NewsTypeDTO>();
                ViewBag.NewsTypeName = newsType.Title;
            }
            string url = $"News/PageListUI?page=1&pageSize=5&newtypeId={newsTypeId}";
            res = await _apiCallNews.GetListAsync(url);
            if (res.Result == 1)
            {
                ViewBag.NewsData = res.Data!.data.ToObject<List<NewsDTO>>();
            }

            return View();
        }
    }
}
