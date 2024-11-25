using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using WebTinTucFLIC.Entities;
using WebTinTucFLIC.Helper;
using WebTinTucFLIC.Models;

namespace WebTinTucFLIC.Controllers
{
    public class ImageController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly APICaller<ImageDTO> _apiCall;
        private readonly IWebHostEnvironment _webHostEnvironment;
        Response res = new Response();
        public ImageController(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            _configuration = configuration;
            _apiCall = new APICaller<ImageDTO>(_configuration);
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<int> CreateImage(string imageUrl, int userId)
        {
            try
            {
                var model = new ImageDTO
                {
                    Id = 0,
                    Description = "Hinh anh",
                    ImageUrl = imageUrl
                };
                
                res = await _apiCall.CreateAsync($"Image/Create?userId={userId}", model);
                if (res.Result == 1 && res.Data != null)
                {
                    var image = res.Data!.ToObject<ImageDTO>();
                    return image.Id;
                }
            }
            catch (Exception ex)
            {
                res.Message = $"Có lỗi khi tìm kiếm dữ liệu {ex.Message}. InnerException {ex.InnerException?.Message}";
                return 0;
            }
            return 0;
        }

        [HttpPost]
        public IActionResult UploadImage(IFormFile upload)
        {
            var fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + upload.FileName;
            var path = Path.Combine(Directory.GetCurrentDirectory(),
            _webHostEnvironment.WebRootPath, "uploads", fileName);
            var stream = new FileStream(path, FileMode.Create);
            upload.CopyToAsync(stream);
            return new JsonResult(new
            {
                uploaded = 1,
                fileName = upload.FileName,
                url = "/uploads/" + fileName
            });
        }
    }
}
