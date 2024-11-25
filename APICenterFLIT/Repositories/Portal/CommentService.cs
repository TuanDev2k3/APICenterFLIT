
using System.Linq.Expressions;
using APICenterFlit.Entities;
using APICenterFlit.Helper;
using APICenterFlit.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace APICenterFlit.Repositories.Portal
{
    public interface ICommentService
    {
        Task<Response> GetListAsync();
        Task<Response> EditById(int id);
        Task<Response> Create(CommentDTO model, int userId);
        Task<Response> Update(CommentDTO model, int id, int userId);
        Task<Response> Delete(int id, int userId);
        Task<Response> Search(Expression<Func<Comment, bool>> expression);
        Task<Response> PaginationAsync(int page, int pageSize);
        Task<Response> ListCommentsByNewId(int newId);

    }
    public class CommentService : ICommentService
    {
        private readonly DbnewsContext _db;
        private readonly IMapper _mapper;
        public CommentService(DbnewsContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }
        public async Task<Response> Create(CommentDTO model, int userId)
        {
            Response res = new Response();
            try
            {
                Comment data = _mapper.Map<CommentDTO, Comment>(model);
                int maxId = await _db.Comments.MaxAsync(m => (int?)m.Id) ?? 0;
                data.Id = maxId + 1;
                data.Status = 1;
                data.AccountId = userId;
                data.CommentAt = DateTime.Now;
                data.UpdateAt = DateTime.Now;
                await _db.Comments.AddAsync(data);
                await _db.SaveChangesAsync();

                res.Result = 1;
                res.Status = 201;
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
                var data = await _db.Comments.Where(a => a.Status == 1 && a.Id == id).FirstOrDefaultAsync();
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
                var data = await _db.Comments.Where(a => a.Status == 1 && a.Id == id).FirstOrDefaultAsync();
                if (data != null)
                {
                    CommentDTO model = new CommentDTO();
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
                var data = await _db.Comments.Where(a => a.Status == 1).ToListAsync();
                List<CommentDTO> model = new List<CommentDTO>();
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

        public async Task<Response> ListCommentsByNewId(int newId)
        {
            Response res = new Response();
            try
            {
                var data = await _db.Comments.Include(a => a.Account).Include(b =>  b.Account.Image)
                    .Where(a => a.Status == 1 && a.NewId == newId).OrderByDescending(o => o.CommentAt).ToListAsync();
                if (data != null)
                {
                    var model = new List<CommentDTO>();
                    _mapper.Map(data, model);
                    foreach(var item in data)
                    {
                        var dto = model.SingleOrDefault(p => p.Id == item.Id);
                        if (dto != null)
                        {
                            dto.AccountName = item.Account.Fullname;
                            dto.ImageURL = item.Account.Image.ImageUrl;
                        }
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
                var comment = _db.Comments.Where(x => x.Status == 1).OrderByDescending(o => o.CommentAt);
                var totalCount = await comment.CountAsync();
                var totalPages = (int)Math.Ceiling((decimal)totalCount / pageSize);
                var newsPerPage = await comment.Skip((page - 1) * pageSize).Take(pageSize).Include(a => a.Account).ToListAsync();
                List<CommentDTO> modeldto = new List<CommentDTO>();
                _mapper.Map(newsPerPage, modeldto);
                foreach (var item in modeldto)
                {
                    var findcomment = newsPerPage.FirstOrDefault(a => a.AccountId == item.AccountId);
                    var findUser = await _db.Accounts.Include(a => a.Image).SingleOrDefaultAsync(b => b.Id == item.AccountId);
                    if (findcomment != null)
                    {
                        item.AccountName = findcomment.Account.Fullname;
                        item.ImageURL = findUser?.Image.ImageUrl;
                    }
                    else
                    {
                        item.AccountName = "Ẩn danh";
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

        public async Task<Response> Search(Expression<Func<Comment, bool>> expression)
        {
            Response res = new Response();
            try
            {
                var getItem = await _db.Comments.Where(expression).ToListAsync();
                if (getItem.Any())
                {
                    var itemDto = _mapper.Map<List<CommentDTO>>(getItem);
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


        public async Task<Response> Update(CommentDTO model, int id, int userId)
        {
            Response res = new Response();
            try
            {
                var data = await _db.Comments.Where(a => a.Status == 1 && a.Id == id).FirstOrDefaultAsync();
                if (data != null)
                {
                    _mapper.Map(model, data);
                    data.Id = id;
                    data.UpdateAt = DateTime.Now;
                    data.UpdateBy = userId;
                    _db.Comments.Update(data);
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
