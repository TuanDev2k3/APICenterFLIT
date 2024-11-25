
using System.Collections.Generic;
using System.Linq.Expressions;
using APICenterFlit.Entities;
using APICenterFlit.Helper;
using APICenterFlit.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace APICenterFlit.Repositories.Portal
{
	public interface INewsService
	{
		Task<Response> GetListAsync();
		Task<Response> EditById(int id);
		Task<Response> Create(NewsDTO model, int userId);
		Task<Response> Update(NewsDTO model, int id, int userId);
		Task<Response> Delete(int id, int userId);
		Task<Response> Search(Expression<Func<News, bool>> expression);
		Task<Response> PaginationAsync(int page, int pageSize, int? newtypeId, int? userId, bool? review);
		Task<Response> PageList(int page, int pageSize, int? newtypeId);
		Task<Response> ChapNhanDuyetBai(int id, int userId, int ishot, int homepage);
        Task<Response> TuChoiDuyetBai(int id, int userId);
        Task<Response> GetArticlesCountByMonth(int year);
        Task<Response> Top2HotBetter();
        //Task<Response> NhanBan();
    }
    public class NewsService : INewsService
	{
		private readonly DbnewsContext _db;
		private readonly IMapper _mapper;
		public NewsService(DbnewsContext db, IMapper mapper)
		{
			_db = db;
			_mapper = mapper;
		}
		public async Task<Response> Create(NewsDTO model, int userId)
		{
			Response res = new Response();
			try
			{
				News data = _mapper.Map<NewsDTO, News>(model);
				int maxId = await _db.News.MaxAsync(m => (int?)m.Id) ?? 0;
				data.Id = maxId + 1;
				data.TitleSlug = TittleToSlug.ConvertToSlug(data.Title);
				data.Status = 0;
				data.IsHot = 0;
				data.HomePage = 0;
				data.CreateAt = model.PublishAt;
				data.UpdateAt = DateTime.Now;
				data.UpdateBy = userId;
				data.CreateBy = userId;
				await _db.News.AddAsync(data);
				await _db.SaveChangesAsync();

				res.Result = 1;
				res.Status = 201;
				res.Message = "Thêm dữ liệu thành công !!";
				res.Data = data.Id;
			}
			catch (Exception ex)
			{
				res.Result = 0;
				res.Status = 400;
				res.Message = ex.InnerException?.Message ?? ex.Message;
			}
			return res;
		}

        public async Task<Response> Top2HotBetter()
        {
            Response res = new Response();
            try
            {
                var data = await _db.News.Include(a => a.Image).Include(b => b.NewType).Where(a => a.Status == 1 && a.IsHot == 1)
                    .OrderByDescending(c => c.CreateAt) 
					.Take(2)
					.ToListAsync();
                var model = new List<NewsDTO>();
                _mapper.Map(data, model);
                foreach (var item in data)
                {
                    var itemdto = model.FirstOrDefault(p => p.Id == item.Id);
                    if (itemdto != null)
                    {
                        itemdto.ImageUrl = item.Image.ImageUrl;
                        itemdto.NewsTypeName = item.NewType.Title;
                    }
                }
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

        public async Task<Response> Delete(int id, int userId)
		{
			Response res = new Response();
			try
			{
				var data = await _db.News.Where(a => a.Status != -1 && a.Id == id).FirstOrDefaultAsync();
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
				var data = await _db.News.Include(nt => nt.NewType).Include(i => i.Image).Where(a => a.Status != -1 && a.Id == id).FirstOrDefaultAsync();
				if (data != null)
				{
					NewsDTO model = new NewsDTO();
					_mapper.Map(data, model);
					model.NewsTypeName = data.NewType.Title;
					model.ImageUrl = data.Image.ImageUrl;
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
		public async Task<Response> PaginationAsync(int page, int pageSize, int? newtypeId, int? userId, bool? review)
		{
			Response res = new Response();
			try
			{
				var news = _db.News.Include(nt => nt.NewType).Where(x => x.Status != -1);

                if (newtypeId != null) // the loai
				{
					news = news.Where(x => x.NewTypeId == newtypeId || x.NewType.ParentId == newtypeId);
                }
				if (userId != null) // cho dang bai
				{
					news = news.Where(x => x.CreateBy == userId);
				}
				if (review == true) // cho kiem duyet
				{
					news = news.Where(x => x.Status == 0);
				}
				news = news.OrderByDescending(o => o.CreateAt);

                var totalCount = await news.CountAsync();
				var totalPages = (int)Math.Ceiling((decimal)totalCount / pageSize);
				var newsPerPage = await news.Skip((page - 1) * pageSize).Take(pageSize).Include(i => i.Image).ToListAsync();

                List<NewsDTO> modeldto = new List<NewsDTO>();
                _mapper.Map(newsPerPage, modeldto);
                foreach (var item in newsPerPage)
                {
                    var itemdto = modeldto.FirstOrDefault(p => p.Id == item.Id);
                    if (itemdto != null)
                    {
                        itemdto.NewsTypeName = item.NewType.Title;
						itemdto.ImageUrl = item.Image.ImageUrl;
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
            {//Them Include(i => i.Image)
                var data = await _db.News.Include(nt => nt.NewType).Include(i => i.Image).Where(a => a.Status != -1).ToListAsync();
                List<NewsDTO> model = new List<NewsDTO>();
                _mapper.Map(data, model);
                foreach (var item in data)
                {
                    var itemdto = model.FirstOrDefault(p => p.Id == item.Id);
                    if (itemdto != null)
                    {
                        itemdto.NewsTypeName = item.NewType.Title;
                        //Them Image
                        itemdto.ImageUrl = item.Image != null ? item.Image.ImageUrl : "/images/default.jpg";
                    }
                }
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
        //public async Task<Response> GetListAsync()
        //{
        //	Response res = new Response();
        //	try
        //	{
        //		var data = await _db.News.Include(nt => nt.NewType).Where(a => a.Status != -1).ToListAsync();
        //		List<NewsDTO> model = new List<NewsDTO>();
        //		_mapper.Map(data, model);
        //		foreach (var item in data)
        //		{
        //			var itemdto = model.FirstOrDefault(p => p.Id == item.Id);
        //			if (itemdto != null)
        //			{
        //				itemdto.NewsTypeName = item.NewType.Title;
        //			}
        //		}
        //		res.Result = 1;
        //		res.Status = 200;
        //		res.Message = "Lấy dữ liệu thành công";
        //		res.Data = model;
        //	}
        //	catch (Exception ex)
        //	{
        //		res.Status = 400;
        //		res.Message = ex.InnerException?.Message ?? ex.Message;
        //	}
        //	return res;
        //}

        public async Task<Response> Search(Expression<Func<News, bool>> expression)
		{
			Response res = new Response();
			try
			{
				var getItem = await _db.News.Where(expression).ToListAsync();
				if (getItem.Any())
				{
					var itemDto = _mapper.Map<List<NewsDTO>>(getItem);
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

		public async Task<Response> Update(NewsDTO model, int id, int userId)
		{
			Response res = new Response();
			try
			{
				var data = await _db.News.Where(a => a.Status != -1 && a.Id == id).FirstOrDefaultAsync();
				if (data != null)
				{
					var status = data.Status; // lay va giu trang thai hien tai
					_mapper.Map(model, data);
					data.Id = id;
					data.Status = status;
                    data.TitleSlug = TittleToSlug.ConvertToSlug(data.Title);
                    data.UpdateAt = DateTime.Now;
					data.UpdateBy = userId;
					_db.News.Update(data);
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

        public async Task<Response> ChapNhanDuyetBai(int id, int userId, int ishot, int homepage)
        {
            Response res = new Response();
            try
            {
                var data = await _db.News.Where(a => a.Status == 0 && a.Id == id).FirstOrDefaultAsync();
                if (data != null)
                {
					if (data.PublishAt <= DateTime.Now)
					{
						data.Status = 1; // Xuấtt bản bài viết.
						data.PublishAt = DateTime.Now;
                    }
					else
					{
						data.Status = 3; // Bài viết đã duyệt và chờ ngày xuấtt bản
					}
                   
					data.IsHot = ishot;
					data.HomePage = homepage;
                    data.UpdateAt = DateTime.Now;
                    data.UpdateBy = userId;

                    await _db.SaveChangesAsync();
                    res.Result = 1;
                    res.Status = 200;
                    res.Message = "Duyệt bài thành công !!";
                }
                else
                {
                    res.Status = 401;
                    res.Message = "Không tìm thấy dữ liệu !!";
                }
            }
            catch (Exception ex)
            {
                res.Status = 400;
                res.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return res;
        }

        public async Task<Response> TuChoiDuyetBai(int id, int userId)
        {
            Response res = new Response();
            try
            {
                var data = await _db.News.Where(a => a.Status == 0 && a.Id == id).FirstOrDefaultAsync();
                if (data != null)
                {
                    data.Status = 2;
                    data.UpdateAt = DateTime.Now;
                    data.UpdateBy = userId;
                    await _db.SaveChangesAsync();
                    res.Result = 1;
                    res.Status = 200;
                    res.Message = "Từ chối duyệt bài thành công !!";
                }
                else
                {
                    res.Status = 401;
                    res.Message = "Không tìm thấy dữ liệu !!";
                }
            }
            catch (Exception ex)
            {
                res.Status = 400;
                res.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return res;
        }

		// Viet them
        public async Task<Response> GetArticlesCountByMonth(int year)
        {
            Response res = new Response();
            try
            {
                var result = await _db.News
					.Where(a => a.CreateAt.Year == year) 
					.GroupBy(a => a.CreateAt.Month) 
					.Select(g => new
					{
						Month = g.Key,
						Count = g.Count()
					})
					.OrderBy(g => g.Month)
					.ToListAsync();
                res.Result = 1;
                res.Status = 200;
                res.Message = "SUCCESS";
				res.Data = result;
            }
            catch (Exception ex)
            {
                res.Status = 400;
                res.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return res;
        }

        public async Task<Response> PageList(int page, int pageSize, int? newtypeId)
        {
            Response res = new Response();
            try
            {
                var news = _db.News.Include(nt => nt.NewType).Where(x => x.Status == 1);

                if (newtypeId != null) // the loai
                {
                    news = news.Where(x => x.NewTypeId == newtypeId || x.NewType.ParentId == newtypeId);
                }
                news = news.OrderByDescending(o => o.PublishAt);

                var totalCount = await news.CountAsync();
                var totalPages = (int)Math.Ceiling((decimal)totalCount / pageSize);
                var newsPerPage = await news.Skip((page - 1) * pageSize).Take(pageSize).Include(i => i.Image).ToListAsync();

                List<NewsDTO> modeldto = new List<NewsDTO>();
                _mapper.Map(newsPerPage, modeldto);
                foreach (var item in newsPerPage)
                {
                    var itemdto = modeldto.FirstOrDefault(p => p.Id == item.Id);
                    if (itemdto != null)
                    {
                        itemdto.NewsTypeName = item.NewType.Title;
                        itemdto.ImageUrl = item.Image.ImageUrl;
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

        //public async Task<Response> NhanBan()
        //{
        //    Response res = new Response();
        //    try
        //    {
        //        // 1. Lấy tất cả các bản ghi hiện có từ bảng News
        //        var existingNews = await _db.News.AsNoTracking().ToListAsync();

        //        // 2. Danh sách chứa các bản ghi mới
        //        var newNewsList = new List<News>();

        //        foreach (var news in existingNews)
        //        {
        //            for (int i = 0; i < 20; i++)
        //            {
        //                int maxId = await _db.News.MaxAsync(m => (int?)m.Id) ?? 0;
        //                // 3. Tạo bản sao của từng bản ghi
        //                var newNews = new News
        //                {
        //                    Id = maxId + 1,
        //                    NewTypeId = news.NewTypeId,
        //                    Title = news.Title + $" (Copy {i + 1})", // Đổi tên để tránh trùng lặp
        //                    TitleSlug = news.TitleSlug + $"-copy-{i + 1}", // Đổi slug để tránh trùng lặp
        //                    Description = news.Description,
        //                    Detail = news.Detail,
        //                    ImageId = news.ImageId,
        //                    PublishAt = news.PublishAt.AddMinutes(i), // Thêm khác biệt nhỏ về thời gian
        //                    IsHot = news.IsHot,
        //                    Status = news.Status,
        //                    CreateAt = news.CreateAt, // Ngày tạo mới
        //                    CreateBy = news.CreateBy,
        //                    UpdateAt = news.UpdateAt, // Ngày cập nhật mới
        //                    UpdateBy = news.UpdateBy,
        //                    HomePage = news.HomePage
        //                };

        //                /// newNewsList.Add(newNews);
        //                _db.News.Add(newNews);
        //                await _db.SaveChangesAsync();
        //            }
        //        }

        //        // 4. Thêm dữ liệu mới vào database
        //        //await _db.News.AddRangeAsync(newNewsList);
        //        await _db.SaveChangesAsync();

        //        res.Status = 200;
        //        res.Result = 1;
        //        res.Message = "Add dữ liệu thành công";
        //    }
        //    catch (Exception ex)
        //    {
        //        res.Status = 400;
        //        res.Result = 0;
        //        res.Message = $"Có lỗi khi tìm kiếm dữ liệu {ex.Message}. InnerException {ex.InnerException?.Message}";
        //    }
        //    return res;
        //}
    }
}
