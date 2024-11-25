
using System.Linq.Expressions;
using APICenterFlit.Entities;
using APICenterFlit.Helper;
using APICenterFlit.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace APICenterFlit.Repositories.Portal
{
    public interface IImageService
    {
        Task<Response> GetListAsync();
        Task<Response> EditById(int id);
        Task<Response> Create(ImageDTO model, int userId);
        Task<Response> Update(ImageDTO model, int id, int userId);
        Task<Response> Delete(int id, int userId);
        Task<Response> Search(Expression<Func<Image, bool>> expression);

    }
    public class ImageService : IImageService
    {
        private readonly DbnewsContext _db;
        private readonly IMapper _mapper;
        public ImageService(DbnewsContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }
        public async Task<Response> Create(ImageDTO model, int userId)
        {
            Response res = new Response();
            try
            {
                Image data = _mapper.Map<ImageDTO, Image>(model);
                int maxId = await _db.Images.MaxAsync(m => (int?)m.Id) ?? 0;
                data.Id = maxId + 1;
                data.Status = 1;
                data.CreateAt = DateTime.Now;
                data.CreateBy = userId;
                await _db.Images.AddAsync(data);
                await _db.SaveChangesAsync();

                res.Result = 1;
                res.Status = 201;
                res.Message = "Thêm dữ liệu thành công !!";
                res.Data = data;
            }
            catch (Exception ex)
            {
                res.Status = 400;
                res.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return res;
        }

        public async Task<Response> Delete(int id, int userId)
        {
            Response res = new Response();
            try
            {
                var data = await _db.Images.Where(a => a.Status == 1 && a.Id == id).FirstOrDefaultAsync();
                if (data != null)
                {
                    data.Status = -1;
                    data.UpdateAt = DateTime.Now;
                    data.UpdateBy = userId;
                    await _db.SaveChangesAsync();
                    res.Result = 1;
                    res.Status = 200;
                    res.Message = "Xóa dữ liệu thành công !!";
                }
                else
                {
                    res.Status = 401;
                    res.Message = "Không tìm thấy dữ liệu cần xóa !!";
                }
            }
            catch (Exception ex)
            {
                res.Status = 400;
                res.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return res;
        }

        public async Task<Response> EditById(int id)
        {
            Response res = new Response();
            try
            {
                var data = await _db.Images.Where(a => a.Status == 1 && a.Id == id).FirstOrDefaultAsync();
                if (data != null)
                {
                    ImageDTO model = new ImageDTO();
                    _mapper.Map(data, model);
                    res.Result = 1;
                    res.Status = 200;
                    res.Message = "Lấy dữ liệu thành công !!";
                    res.Data = model;
                }
                else
                {
                    res.Status = 401;
                    res.Message = "Không tìm thấy dữ liệu cần tìm !!";
                }
            }
            catch (Exception ex)
            {
                res.Status = 400;
                res.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return res;
        }

        public async Task<Response> GetListAsync()
        {
            Response res = new Response();
            try
            {
                var data = await _db.Images.Where(a => a.Status == 1).ToListAsync();
                List<ImageDTO> model = new List<ImageDTO>();
                _mapper.Map(data, model);
                res.Result = 1;
                res.Status = 200;
                res.Message = "Lấy dữ liệu thành công";
                res.Data = model;
            }
            catch (Exception ex)
            {
                res.Status = 400;
                res.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return res;
        }

        public async Task<Response> Search(Expression<Func<Image, bool>> expression)
        {
            Response res = new Response();
            try
            {
                var getItem = await _db.Images.Where(expression).ToListAsync();
                if (getItem.Any())
                {
                    var itemDto = _mapper.Map<List<ImageDTO>>(getItem);
                    res.Result = 1;
                    res.Status = 200;
                    res.Message = "Lấy dữ liệu thành công";
                    res.Data = itemDto;
                }
                else
                {
                    res.Status = 401;
                    res.Message = "Tìm kiếm dữ liệu không thành công!!";
                }
            }
            catch (Exception ex)
            {
                res.Status = 400;
                res.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return res;
        }

        public async Task<Response> Update(ImageDTO model, int id, int userId)
        {
            Response res = new Response();
            try
            {
                var data = await _db.Images.Where(a => a.Status == 1 && a.Id == id).FirstOrDefaultAsync();
                if (data != null)
                {
                    _mapper.Map(model, data);
                    data.Id = id;
                    data.UpdateAt = DateTime.Now;
                    data.UpdateBy = userId;
                    _db.Images.Update(data);
                    await _db.SaveChangesAsync();
                    res.Result = 1;
                    res.Status = 204;
                    res.Message = "Sửa dữ liệu thành công !!";
                }
                else
                {
                    res.Status = 401;
                    res.Message = "Không tìm thấy dữ liệu cần sửa !!";
                }
            }
            catch (Exception ex)
            {
                res.Status = 400;
                res.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return res;
        }
    }
}
