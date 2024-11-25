
using System.Linq.Expressions;
using APICenterFlit.Entities;
using APICenterFlit.Helper;
using APICenterFlit.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace APICenterFlit.Repositories.Portal
{
	public interface INewsTypeService
	{
		Task<Response> GetListAsync();
		Task<Response> EditById(int id);
		Task<Response> Create(NewsTypeDTO model, int userId);
		Task<Response> Update(NewsTypeDTO model, int id, int userId);
		Task<Response> Delete(int id, int userId);
		Task<Response> Search(Expression<Func<NewsType, bool>> expression);
		Task<Response> PaginationAsync(int page, int pageSize);
		Task<Response> SelectDropDown();

	}
	public class NewsTypeService : INewsTypeService
	{
		private readonly DbnewsContext _db;
		private readonly IMapper _mapper;
		public NewsTypeService(DbnewsContext db, IMapper mapper)
		{
			_db = db;
			_mapper = mapper;
		}
		public async Task<Response> Create(NewsTypeDTO model, int userId)
		{
			Response res = new Response();
			try
			{
				NewsType data = _mapper.Map<NewsTypeDTO, NewsType>(model);
				int maxId = await _db.NewsTypes.MaxAsync(m => (int?)m.Id) ?? 0;
				data.Id = maxId + 1;
                data.TitleSlug = TittleToSlug.ConvertToSlug(data.Title);
                data.Status = 1;
                data.CreateAt = DateTime.Now;
                data.UpdateAt = DateTime.Now;
                data.UpdateBy = userId;
                data.CreateBy = userId;
                await _db.NewsTypes.AddAsync(data);
				await _db.SaveChangesAsync();

				res.Result = 1;
				res.Status = 201;
				res.Data = data.Id;
				res.Message = "Thêm dữ liệu thành công !!";
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
				var data = await _db.NewsTypes.Where(a => a.Status == 1 && a.Id == id).FirstOrDefaultAsync();
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
				var data = await _db.NewsTypes.Include(a => a.Image).Where(a => a.Status == 1 && a.Id == id).FirstOrDefaultAsync();
				if (data != null)
				{
					NewsTypeDTO model = new NewsTypeDTO();
					_mapper.Map(data, model);
                    model.ImageUrl = data.Image.ImageUrl;
                    var findParent = await _db.NewsTypes.FindAsync(model.ParentId);
					if (findParent != null)
					{
                        model.ParentName = findParent.Title;
                    }
					else
					{
                        model.ParentName = "Không có";
                    }
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
		public async Task<Response> PaginationAsync(int page, int pageSize)
		{
			Response res = new Response();
			try
			{
				var newstype =  _db.NewsTypes.Where(x => x.Status == 1).OrderByDescending(o => o.UpdateAt);
				var totalCount = await newstype.CountAsync();
				var totalPages = (int)Math.Ceiling((decimal)totalCount / pageSize);
				var newsPerPage = await newstype.Skip((page - 1) * pageSize).Take(pageSize).Include(a => a.Image).ToListAsync();
                List<NewsTypeDTO> modeldto = new List<NewsTypeDTO>();
                _mapper.Map(newsPerPage, modeldto);
                foreach (var item in modeldto)
                {
                    var findParent = newsPerPage.FirstOrDefault(a => a.Id == item.ParentId);
					var itemdata = newsPerPage.FirstOrDefault(p => p.Id == item.Id);
                    if (findParent != null)
                    {
                        item.ParentName = findParent.Title;
                    }
					else
					{
						item.ParentName = "Không có";
					}

					if (itemdata != null)
					{
						item.ImageUrl = itemdata.Image.ImageUrl;
					}
                }

                var list = new
                {
                    data = modeldto,
                    count = totalPages
                };
                res.Data = list;
                res.Status = 200;
				res.Result = 1;
				res.Message = "Tìm kiếm dữ liệu thành công";
			}
			catch (Exception ex)
			{
				res.Status = 400;
				res.Result = 0;
				res.Message = $"Có lỗi khi tìm kiếm dữ liệu {ex.Message}. InnerException {ex.InnerException?.Message}";
			}
			return res;
		}
		public async Task<Response> GetListAsync()
		{
			Response res = new Response();
			try
			{
				var data = await _db.NewsTypes.Where(a => a.Status == 1).ToListAsync();
				List<NewsTypeDTO> model = new List<NewsTypeDTO>();
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

		public async Task<Response> Search(Expression<Func<NewsType, bool>> expression)
		{
			Response res = new Response();
			try
			{
				var getItem = await _db.NewsTypes.Where(expression).ToListAsync();
				if (getItem.Any())
				{
					var itemDto = _mapper.Map<List<NewsTypeDTO>>(getItem);
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

		public async Task<Response> Update(NewsTypeDTO model, int id, int userId)
		{
			Response res = new Response();
			try
			{
				var data = await _db.NewsTypes.Where(a => a.Status == 1 && a.Id == id).FirstOrDefaultAsync();
				if (data != null)
				{
					_mapper.Map(model, data);
					data.Id = id;
                    data.TitleSlug = TittleToSlug.ConvertToSlug(data.Title);
                    data.UpdateAt = DateTime.Now;
					data.UpdateBy = userId;
					_db.NewsTypes.Update(data);
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

        public async Task<Response> SelectDropDown()
        {
            Response res = new Response();
            try
            {
				var data = await _db.NewsTypes.Where(a => a.Status == 1)
					.Select(p => new { p.Id, p.Title , p.ParentId})
					.ToListAsync();
                res.Result = 1;
                res.Status = 200;
                res.Message = "Lấy dữ liệu thành công";
                res.Data = data;
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
