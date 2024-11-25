
using System.Linq.Expressions;
using APICenterFlit.Entities;
using APICenterFlit.Helper;
using APICenterFlit.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace APICenterFlit.Repositories.Portal
{
	public interface IAccountTypeService
	{
		Task<Response> GetListAsync();
		Task<Response> EditById(int id);
		Task<Response> Create(AccountTypeDTO model, int userId);
		Task<Response> Update(AccountTypeDTO model, int id, int userId);
		Task<Response> Delete(int id, int userId);
		Task<Response> Search(Expression<Func<AccountType, bool>> expression);
        Task<Response> SelectDropDown();
		
    }
	public class AccountTypeService : IAccountTypeService
	{
		private readonly DbnewsContext _db;
		private readonly IMapper _mapper;
		public AccountTypeService(DbnewsContext db, IMapper mapper)
		{
			_db = db;
			_mapper = mapper;
		}
		public async Task<Response> Create(AccountTypeDTO model, int userId)
		{
			Response res = new Response();
			try
			{
				AccountType data = _mapper.Map<AccountTypeDTO, AccountType>(model);
				int maxId = await _db.AccountTypes.MaxAsync(m => (int?)m.Id) ?? 0;
				data.Id = maxId + 1;
				data.Status = 1;
				data.CreateAt = DateTime.Now;
				data.CreateBy = userId;
				data.UpdateAt = DateTime.Now;
				data.UpdateBy = userId;
				await _db.AccountTypes.AddAsync(data);
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
		public async Task<Response> Search(Expression<Func<AccountType, bool>> expression)
		{
			Response res = new Response();
			try
			{
				var getItem = await _db.AccountTypes.Where(expression).ToListAsync();
				if (getItem.Any())
				{
					var itemDto = _mapper.Map<List<AccountTypeDTO>>(getItem);
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
		public async Task<Response> Delete(int id, int userId)
		{
			Response res = new Response();
			try
			{
				var data = await _db.AccountTypes.Where(a => a.Status == 1 && a.Id == id).FirstOrDefaultAsync();
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
				var data = await _db.AccountTypes.Where(a => a.Status == 1 && a.Id == id).FirstOrDefaultAsync();
				if (data != null)
				{
					AccountTypeDTO model = new AccountTypeDTO();
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
				var data = await _db.AccountTypes.Where(a => a.Status == 1).ToListAsync();
				List<AccountTypeDTO> model = new List<AccountTypeDTO>();
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

		public async Task<Response> Update(AccountTypeDTO model, int id, int userId)
		{
			Response res = new Response();
			try
			{
				var data = await _db.AccountTypes.Where(a => a.Status == 1 && a.Id == id).FirstOrDefaultAsync();

				if (data != null)
				{
					_mapper.Map(model, data);
					data.Id = id;
					data.UpdateAt = DateTime.Now;
					data.UpdateBy = userId;
					_db.AccountTypes.Update(data);
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
                var data = await _db.AccountTypes.Where(a => a.Status == 1)
                    .Select(p => new { p.Id, p.Name })
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
