using APICenterFlit.Entities;
using APICenterFlit.Helper;
using APICenterFlit.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace APICenterFlit.Repositories.Users
{
    public interface IAccessLogService
    {
        Task<Response> Create(AccessLogDTO model);
        Task<Response> PaginationAsync(int page, int pageSize);

    }
    public class AccessLogService : IAccessLogService
    {
        private readonly DbnewsContext _db;
        private readonly IMapper _mapper;
        public AccessLogService(DbnewsContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }
        public async Task<Response> Create(AccessLogDTO model)
        {
            Response res = new Response();
            try
            {
                var data = _mapper.Map<AccessLogDTO, AccessLog>(model);
                int maxId = await _db.AccessLogs.MaxAsync(m => (int?)m.Id) ?? 0;
                data.Id = maxId + 1;
                var vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                data.Timer = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone); // Giờ VN
                await _db.AccessLogs.AddAsync(data);
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


        public async Task<Response> PaginationAsync(int page, int pageSize)
        {
            Response res = new Response();
            try
            {
                var logs = _db.AccessLogs.OrderByDescending(o => o.Timer);
                var totalCount = await logs.CountAsync();
                var totalPages = (int)Math.Ceiling((decimal)totalCount / pageSize);
                var logPerPage = await logs.Skip((page - 1) * pageSize).Take(pageSize).Include(a => a.Account).ToListAsync();
                var modeldto = new List<AccessLogDTO>();
                _mapper.Map(logPerPage, modeldto);
                foreach (var item in modeldto)
                {
                    var findParent = logPerPage.FirstOrDefault(a => a.Id == item.Id);
                    if (findParent != null)
                    {
                        item.AccountName = findParent.Account.UserName;
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
    }
}
