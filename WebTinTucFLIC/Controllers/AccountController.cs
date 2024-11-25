using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using WebTinTucFLIC.Helper;
using WebTinTucFLIC.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Reflection;
using WebTinTucFLIC.Entities;
using Microsoft.AspNetCore.Authorization;
using System.Net;
using System.Security.Principal;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Xml.Linq;

namespace WebTinTucFLIC.Controllers
{
    public class AccountController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly APICaller<AccountDTO> _apiCall;
        private readonly APICaller<LoginModal> _apiCallLogin;
        private readonly APICaller<ChangePasswordViewModel> _apiCallPass;
        private readonly ImageController _imageController;
        private readonly AccessLogController _accessLogController;
        Response res = new Response();
        public AccountController(IConfiguration configuration, ImageController imageController, AccessLogController accessLogController)
        {
            _configuration = configuration;
            _apiCall = new APICaller<AccountDTO>(_configuration);
            _apiCallLogin = new APICaller<LoginModal>(_configuration);
            _apiCallPass = new APICaller<ChangePasswordViewModel>(_configuration);
            _imageController = imageController;
            _accessLogController = accessLogController;
        }

        public async Task<List<AccountTypeDTO>> SelectDropDown()
        {
            res = await _apiCall.GetListAsync($"AccountType/SelectDropDown");
            if (res.Result == 1)
            {
                var listnewstype = res.Data!.ToObject<List<AccountTypeDTO>>();
                return listnewstype;
            }
            return null!;
        }

        [Authorize(Roles = "ADM")]
        public async Task<IActionResult> P_List(int? page = 1, int? pageSize = 10, int? accountTypeId = null, int? status = 0)
        {
            string url = $"/Account/Pagedlist?page={page}&pageSize={pageSize}";
            if (status != 0)
            {
                url += $"&status={status}";
            }
            if (accountTypeId != null)
            {
                url += $"&accountTypeId={accountTypeId}";
            }
            res = await _apiCall.GetListAsync(url);

            if (res.Result == 1)
            {
                var danhsach = res.Data!.data.ToObject<List<AccountDTO>>();
                ViewBag.PageTotal = Convert.ToInt32(res.Data!.count);
                ViewBag.CurrentPage = page;
                ViewBag.DsAccountType = await SelectDropDown();
                ViewBag.SelectedAccountType = accountTypeId;
                return View(danhsach);
            }
            else
            {
                return View();
            }
        }

        [HttpGet("/Account/LoginPage")]
        public IActionResult LoginPage(string? returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost("/Account/Login")]
        public async Task<IActionResult> Login(LoginModal modal, string? returnUrl)
        {

            // Vì cũng truyền vào 1 model giống Create nên dùng luôn ( tránh viết lại)
            res = await _apiCallLogin.CreateAsync($"/Account/Login", modal);
            if (res.Result == 1)
            {
                var account = res.Data!.ToObject<AccountDTO>();
                var claims = new List<Claim>
                            {
                                new Claim("UserId", account.Id.ToString()),
                                new Claim(ClaimTypes.Email, account.Email),
                                new Claim(ClaimTypes.Name, account.Fullname),
                                new Claim("Username", account.UserName),
                                new Claim("Phone", account.Phone),
                                new Claim("Image", account.ImageUrl),
                                new Claim(ClaimTypes.Role, account.AccountCode)
                            };
                var claimIdentity = new ClaimsIdentity(claims, "login");
                var claimPrincipal = new ClaimsPrincipal(claimIdentity);
                await HttpContext.SignInAsync(claimPrincipal);

                // Luu nhat ki
                var access = new AccessLog
                {
                    AccountId = account.Id,
                    Description = $"{account.Fullname} vừa đăng nhập bằng username và mật khẩu",
                    DeviceName = _accessLogController.GetDeviceName(HttpContext.Request.Headers["User-Agent"].ToString()),
                    BrowersName = _accessLogController.GetBrowserName(HttpContext.Request.Headers["User-Agent"].ToString()),
                    Timer = DateTime.Now,
                    IpAddress = HttpContext.Connection.RemoteIpAddress!.ToString()
                };
                await _accessLogController.CreateAccessLog(access);

                if (Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                else
                {
                    if (account.AccountCode == "USR")
                    {
                        return Redirect("/News/Index");
                    }
                    return Redirect("/Home/Dashboad");
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, res.Message!);
                return View("LoginPage");
            }
        }

        [HttpGet("/Account/LoginWithGoogle")]
        public async Task<IActionResult> LoginWithGoogle()
        {
            return Challenge(new AuthenticationProperties { RedirectUri = "/Account/SignInGoogle" }, GoogleDefaults.AuthenticationScheme);
        }

        public async Task<IActionResult> SignInGoogle()
        {
            Response resGG = new Response();
            var result = await HttpContext.AuthenticateAsync("Google");
            if (result.Succeeded)
            {
                var claim = (ClaimsIdentity)result.Principal.Identity!;
                var data = new AccountDTO
                {
                    Fullname = claim.FindFirst(ClaimTypes.Name)!.Value,
                    Password = "",
                    Email = claim.FindFirst(ClaimTypes.Email)!.Value,
                    UserName = claim.FindFirst(ClaimTypes.Email)!.Value,
                    Phone = "",
                    AccountType = 1,
                    ImageId = 10,
                    AddressId = 1
                };
                resGG = await _apiCall.GetListAsync($"/Account/IsLockedGoogle?email={data.Email}");
                if (resGG.Result == 1 && resGG.Data == true)
                {
                    ModelState.AddModelError(string.Empty, "Tài khoản Google của bạn đã bị khóa.");
                    return SignOut(new AuthenticationProperties { RedirectUri = "/Account/LoginPage" }, "Cookies");
                }
                res = await _apiCall.CreateAsync($"Account/Create?userId=0", data);
                claim.AddClaim(new Claim("UserId", res.Data!.ToString()));
                claim.AddClaim(new Claim(ClaimTypes.Role, "USR"));
                var claimsPrincipal = new ClaimsPrincipal(claim);
                await HttpContext.SignInAsync(claimsPrincipal);

                // Luu nhat ki
                var access = new AccessLog
                {
                    AccountId = Convert.ToInt32(res.Data),
                    Description = $"{data.Fullname} vừa đăng nhập bằng tài khoản Google",
                    DeviceName = _accessLogController.GetDeviceName(HttpContext.Request.Headers["User-Agent"].ToString()),
                    BrowersName = _accessLogController.GetBrowserName(HttpContext.Request.Headers["User-Agent"].ToString()),
                    Timer = DateTime.Now,
                    IpAddress = HttpContext.Connection.RemoteIpAddress!.ToString()
                };
                await _accessLogController.CreateAccessLog(access);
                return RedirectToAction("Index", "News");
            }
            else
            {
                return RedirectToAction("LoginPage", "Account");
            }
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            // Nhat ki hd
            var userId = Convert.ToInt32(HttpContext.User.FindFirst("UserId")?.Value!);
            var name = User.FindFirst(ClaimTypes.Name)?.Value;
            var access = new AccessLog
            {
                AccountId = userId,
                Description = $"{name} vừa đăng xuất tài khoản khỏi hệ thống",
                DeviceName = _accessLogController.GetDeviceName(HttpContext.Request.Headers["User-Agent"].ToString()),
                BrowersName = _accessLogController.GetBrowserName(HttpContext.Request.Headers["User-Agent"].ToString()),
                Timer = DateTime.Now,
                IpAddress = HttpContext.Connection.RemoteIpAddress!.ToString()
            };
            await _accessLogController.CreateAccessLog(access);

            // Đăng xuất người dùng
            return SignOut(new AuthenticationProperties { RedirectUri = "/Account/LoginPage" }, "Cookies");
        }

        [Authorize]
        public IActionResult Profile()
        {
            if (User.Identity!.IsAuthenticated)
            {
                var userName = User.FindFirst(ClaimTypes.Name)?.Value;
                var email = User.FindFirst(ClaimTypes.Email)?.Value;
                var phone = User.FindFirst("Phone")?.Value;
                var profilePicture = User.FindFirst("Image")?.Value ?? "Logo.jpg";

                ViewBag.Name = userName;
                ViewBag.Email = email;
                ViewBag.Phone = phone;
                ViewBag.ProfilePicture = profilePicture;
            }

            return View();
        }

        [HttpGet]
        [Authorize(Roles = "ADM")]
        public async Task<IActionResult> SignUpPage(string? returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.DsAccountType = await SelectDropDown();
            return View();
        }

        [HttpPost("/Account/SignUpPage")]
        [Authorize(Roles = "ADM")]
        public async Task<IActionResult> SignUpPage(AccountDTO modal, string? returnUrl, IFormFile imageFile)
        {
            try
            {
                var userId = int.Parse(User.FindFirst("UserId")?.Value!);
                var name = User.FindFirst(ClaimTypes.Name)?.Value;
                if (imageFile != null && imageFile.Length > 0)
                {
                    modal.ImageUrl = myUltilities.UploadImage(imageFile, "accounts");
                    modal.ImageId = await _imageController.CreateImage(modal.ImageUrl, userId);
                }
                else
                {
                    modal.ImageId = 10;
                    modal.ImageUrl = "Logo.jpg";
                }

                modal.AddressId = 1;
                res = await _apiCall.CreateAsync($"Account/Create?userId={userId}", modal);
                if (res.Result == 1)
                {
                    // Luu nhat ki
                    var access = new AccessLog
                    {
                        AccountId = userId,
                        Description = $"{name} vừa tạo tài khoản mới với Id = {res.Data}",
                        DeviceName = _accessLogController.GetDeviceName(HttpContext.Request.Headers["User-Agent"].ToString()),
                        BrowersName = _accessLogController.GetBrowserName(HttpContext.Request.Headers["User-Agent"].ToString()),
                        Timer = DateTime.Now,
                        IpAddress = HttpContext.Connection.RemoteIpAddress!.ToString()
                    };
                    await _accessLogController.CreateAccessLog(access);
                    return Redirect("/Account/P_List");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, res.Message!);
                    return View("SignUpPage");
                }
            }
            catch (Exception ex)
            {
                res.Message = $"Có lỗi khi tìm kiếm dữ liệu {ex.Message}. InnerException {ex.InnerException?.Message}";
            }
            return View("SignUpPage");
        }

        [Authorize(Roles = "ADM")]
        public async Task<IActionResult> Lock(int id)
        {
            try
            {
                var userId = int.Parse(User.FindFirst("UserId")?.Value!);
                var name = User.FindFirst(ClaimTypes.Name)?.Value;
                res = await _apiCall.DeleteAsync($"/Account/Lock?id={id}&userId={userId}");
                if (res.Result == 1)
                {
                    var access = new AccessLog
                    {
                        AccountId = userId,
                        Description = $"{name} vừa khóa hoặc mở khóa vĩnh viễn tài khoản với Id = {id}",
                        DeviceName = _accessLogController.GetDeviceName(HttpContext.Request.Headers["User-Agent"].ToString()),
                        BrowersName = _accessLogController.GetBrowserName(HttpContext.Request.Headers["User-Agent"].ToString()),
                        Timer = DateTime.Now,
                        IpAddress = HttpContext.Connection.RemoteIpAddress!.ToString()
                    };
                    await _accessLogController.CreateAccessLog(access);
                    return Redirect("/Account/P_List");
                }
            }
            catch (Exception ex)
            {
                res.Message = $"Có lỗi khi tìm kiếm dữ liệu {ex.Message}. InnerException {ex.InnerException?.Message}";
            }
            return Redirect("/Account/P_List");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel modal)
        {
            if (ModelState.IsValid)
            {
                var id = int.Parse(User.FindFirst("UserId")?.Value!);
                var name = User.FindFirst(ClaimTypes.Name)?.Value;
                res = await _apiCallPass.UpdateAsync($"Account/ChangePassword?id={id}", modal);
                if (res.Result == 1)
                {
                    ViewBag.Message = "Đổi mật khẩu thành công!";
                    var access = new AccessLog
                    {
                        AccountId = id,
                        Description = $"{name} vừa thực hiện thay đổi mật khẩu",
                        DeviceName = _accessLogController.GetDeviceName(HttpContext.Request.Headers["User-Agent"].ToString()),
                        BrowersName = _accessLogController.GetBrowserName(HttpContext.Request.Headers["User-Agent"].ToString()),
                        Timer = DateTime.Now,
                        IpAddress = HttpContext.Connection.RemoteIpAddress!.ToString()
                    };
                    await _accessLogController.CreateAccessLog(access);
                    return RedirectToAction("Profile");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, res.Message!);
                }
            }
            return View("ChangePassword");
        }

        [Authorize]
        public async Task<IActionResult> AccessDenied(string? returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
    }
}
