using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebTinTucFLIC.Controllers;
using WebTinTucFLIC.Models;

namespace WebTinTucFLIC.ViewComponents
{
    public class FooterViewComponent : ViewComponent
    {

        private readonly NewsTypeController _news;

        public FooterViewComponent(NewsTypeController news)
        {
            _news = news;
        }

        // Thay đổi tên phương thức thành InvokeAsync
        public async Task<IViewComponentResult> InvokeAsync()
        {
            // Gọi phương thức SelectDropDown và lấy danh sách NewsType
            var items = await _news.SelectDropDown();

            // Chuyển đổi từ List<NewsType> sang List<NewsDTO>
            var newsDTOs = items.Select(nt => new NewsTypeDTO
            {
                Id = nt.Id,
                Title = nt.Title
                // ... thêm các thuộc tính khác nếu cần
            }).ToList(); // Thêm ToList() để chuyển thành List<NewsDTO>

            // Trả về View với dữ liệu DTO
            return View(newsDTOs);
        }
    }
}