using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebTinTucFLIC.Controllers;
using WebTinTucFLIC.Models;

namespace WebTinTucFLIC.ViewComponents
{
    public class GridViewComponent : ViewComponent
    {

        private readonly NewsTypeController _newstype;

        public GridViewComponent(NewsTypeController newsType)
        {
            _newstype = newsType;
        }

        // Thay đổi tên phương thức thành InvokeAsync
        public async Task<IViewComponentResult> InvokeAsync()
        {
            // Gọi phương thức SelectDropDown và lấy danh sách NewsType
            var items = await _newstype.SelectDropDown();

            // Chuyển đổi từ List<NewsType> sang List<NewsTypeDTO>
            var newsTypeDTOs = items.Select(nt => new NewsTypeDTO
            {
                // Gán các thuộc tính từ NewsType sang NewsTypeDTO


                ParentId = nt.ParentId,
                Id = nt.Id,
                Title = nt.Title,
                ParentName = items.FirstOrDefault(x => x.Id == nt.ParentId)?.Title ?? "" // Lấy tên Parent
                // ... thêm các thuộc tính khác nếu cần
            }).ToList();

            // Trả về View với dữ liệu DTO
            return View(newsTypeDTOs);
        }

    }
}