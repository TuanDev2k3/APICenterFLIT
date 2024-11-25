
using System.Collections.Generic;
using System.Linq.Expressions;
using APICenterFlit.Entities;
using APICenterFlit.Helper;
using APICenterFlit.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace APICenterFlit.Repositories.Portal
{
	public interface IAccountService
	{
		Task<Response> GetListAsync();
		Task<Response> EditById(int id);
		Task<Response> Create(AccountDTO model, int userId);
		Task<Response> Update(AccountDTO model, int id, int userId);
		Task<Response> Lock(int id, int userId);
		Task<Response> ChangePassword(ChangePasswordViewModel modal, int id);
		Task<Response> Search(Expression<Func<Account, bool>> expression);
		Task<Response> Login(LoginModal modal);
        Task<Response> PaginationAsync(int page, int pageSize, int? accountTypeId, int? status = 0);
		Task<Response> IsLockedGoogle(string email);
    }
    public class AccountService : IAccountService
	{
		private readonly DbnewsContext _db;
		private readonly IMapper _mapper;
		public AccountService(DbnewsContext db, IMapper mapper)
		{
			_db = db;
			_mapper = mapper;
		}

		public async Task<Response> ChangePassword(ChangePasswordViewModel modal, int id)
		{
			Response res = new Response();
			try
			{
				var data = await _db.Accounts.Where(a => a.Status == 1 && a.Id == id).FirstOrDefaultAsync();
				if (data != null && modal.NewPassword == modal.ConfirmNewPassword 
					&& PasswordHelper.VerifyPassword(modal.CurrentPassword, data.Password))
				{
					data.Password = PasswordHelper.HashPassword(modal.NewPassword);
					data.UpdateAt = DateTime.Now;
					data.UpdateBy = id;
					_db.Accounts.Update(data);
					await _db.SaveChangesAsync();
					res.Result = 1;
					res.Status = 204;
					res.Message = "Thay đổi mật khẩu thành công !!";
				}
				else
				{
					res.Result = 0;
					res.Status = 401;
					res.Message = "Không tìm thấy tài khoản hoặc mật khẩu cũ không chính xác !!";
				}
			}
			catch (Exception ex)
			{
				res.Status = 400;
				res.Message = ex.InnerException?.Message ?? ex.Message;
			}
			return res;
		}

		public async Task<Response> Create(AccountDTO model, int userId)
		{
			Response res = new Response();
			try
			{
                var testEmai = await _db.Accounts.SingleOrDefaultAsync(m => m.Email == model.Email && m.Status == 1);
                var testUsername = await _db.Accounts.SingleOrDefaultAsync(m => m.UserName == model.UserName && m.Status == 1);
				if (testEmai != null || testUsername != null)
				{
                    res.Result = 0;
                    res.Status = 400;
                    res.Message = $"Username: {model.UserName} hoặc Email: {model.Email} đã tồn tại !!";
					res.Data = (testEmai != null) ? testEmai.Id : testUsername?.Id;
					return res;
                }
                Account data = _mapper.Map<AccountDTO, Account>(model);
				int maxId = await _db.Accounts.MaxAsync(m => (int?)m.Id) ?? 0;
				data.Id = maxId + 1;
				data.Password = PasswordHelper.HashPassword(data.Password);
				data.AccountType = (int?)data.AccountType ?? 1; // 1 la mac dinh USR: User
				data.Status = 1;
				data.CreateAt = DateTime.Now;
				data.CreateBy = data.UpdateBy = (userId != 0) ? userId : data.Id;
                data.UpdateAt = DateTime.Now;
                await _db.Accounts.AddAsync(data);
				await _db.SaveChangesAsync();

				res.Result = 1;
				res.Status = 201;
				res.Data = data.Id;
				res.Message = "Thêm dữ liệu thành công !!";
			}
			catch (Exception ex)
			{
				res.Result = 0;
				res.Status = 400;
				res.Message = ex.InnerException?.Message ?? ex.Message;
			}
			return res;
		}
		public async Task<Response> Login(LoginModal modal)
		{
			Response res = new Response();
			try
			{
				var userItem = await _db.Accounts.Include(a => a.AccountTypeNavigation).Include(b => b.Image)
					.SingleOrDefaultAsync(x => x.UserName == modal.UserName && x.Status == 1);

				if (userItem != null && PasswordHelper.VerifyPassword(modal.Password, userItem!.Password))
				{
					var userDto = new AccountDTO();
					_mapper.Map(userItem, userDto);
					userDto.AccountCode = userItem.AccountTypeNavigation.Code;
					userDto.ImageUrl = userItem.Image.ImageUrl;
					res.Status = 200;
					res.Result = 1;
					res.Message = "Đăng nhập thành công";
					res.Data = userDto;
				}
				else
				{
					res.Status = 404;
					res.Result = 0;
					res.Message = "Tài khoản hoặc mật khẩu không chính xác ";
				}
			}
			catch (Exception ex)
			{
				res.Status = 400;
				res.Result = 0;
				res.Message = $"Có lỗi khi tìm kiếm dữ liệu {ex.Message}. InnerException {ex.InnerException?.Message}";
			}
			return res;
		}
		public async Task<Response> Search(Expression<Func<Account, bool>> expression)
		{
			Response res = new Response();
			try
			{
				var getItem = await _db.Accounts.Where(expression).ToListAsync();
				if (getItem.Any())
				{
					var itemDto = _mapper.Map<List<AccountDTO>>(getItem);
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
		public async Task<Response> Lock(int id, int userId)
		{
			Response res = new Response();
			try
			{
				var data = await _db.Accounts.Include(t => t.AccountTypeNavigation).Where(a => a.Id == id && a.AccountTypeNavigation.Code != "ADM").FirstOrDefaultAsync();
				if (data != null)
				{
					data.Status = data.Status * -1;
					data.UpdateAt = DateTime.Now;
					data.UpdateBy = userId;
					await _db.SaveChangesAsync();
					res.Result = 1;
					res.Status = 200;
					res.Message = "Khóa dữ liệu thành công !!";
				}
				else
				{
					res.Status = 401;
					res.Message = "Không tìm thấy tài khoản cần khóa !!";
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
				var data = await _db.Accounts.Where(a => a.Status == 1 && a.Id == id).FirstOrDefaultAsync();
				if (data != null)
				{
					AccountDTO model = new AccountDTO();
					_mapper.Map(data, model);
					model.Password = "?????";
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
				var data = await _db.Accounts.Where(a => a.Status == 1).ToListAsync();
				List<AccountDTO> model = new List<AccountDTO>();
				_mapper.Map(data, model);
				foreach (var account in model)
				{
					account.Password = "?????";
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

		public async Task<Response> Update(AccountDTO model, int id, int userId)
		{
			Response res = new Response();
			try
			{
				var data = await _db.Accounts.Where(a => a.Status == 1 && a.Id == id).FirstOrDefaultAsync();

				if (data != null)
				{
					_mapper.Map(model, data);
					data.Id = id;
					data.Password = PasswordHelper.HashPassword(data.Password);
					data.AccountType = (int?)data.AccountType ?? 2; // 2 la mac dinh USR: User
					data.UpdateAt = DateTime.Now;
					data.UpdateBy = userId;
					_db.Accounts.Update(data);
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

        public async Task<Response> PaginationAsync(int page, int pageSize, int? accountTypeId, int? status = 0)
        {
            Response res = new Response();
            try
            {
                var user = _db.Accounts.Include(nt => nt.AccountTypeNavigation).Where(x => x.Status != 0);

                if (status != 0)
                {
                    user = user.Where(x => x.Status == status);
                }
				if (accountTypeId != null)
				{
                    user = user.Where(x => x.AccountType == accountTypeId);
                }
                user = user.OrderBy(o => o.AccountTypeNavigation.Code);

                var totalCount = await user.CountAsync();
                var totalPages = (int)Math.Ceiling((decimal)totalCount / pageSize);
                var newsPerPage = await user.Skip((page - 1) * pageSize).Take(pageSize).Include(i => i.Image).ToListAsync();

                List<AccountDTO> modeldto = new List<AccountDTO>();
                _mapper.Map(newsPerPage, modeldto);
                foreach (var item in newsPerPage)
                {
                    var itemdto = modeldto.FirstOrDefault(p => p.Id == item.Id);
                    if (itemdto != null)
                    {
                        itemdto.AccountCode = item.AccountTypeNavigation.Code;
						itemdto.AccountTypeName = item.AccountTypeNavigation.Name;
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

        public async Task<Response> IsLockedGoogle(string email)
        {
            Response res = new Response();
            try
			{
				var user = await _db.Accounts.SingleOrDefaultAsync(a => a.Email == email);
                if (user != null && user.Status != 1)
                {
                    res.Data = true;
                    res.Status = 200;
                    res.Result = 1;
                    res.Message = "Tài khoản GG đã bị khóa";
                }
				else
				{
                    res.Data = false;
                    res.Status = 200;
                    res.Result = 1;
                    res.Message = "Tài khoản còn hoạt động";
                }
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
