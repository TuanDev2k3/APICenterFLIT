using System.ComponentModel.DataAnnotations;

namespace WebTinTucFLIC.Models
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Không được để trống")]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "Không được để trống")]
        [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Không được để trống")]
        [Compare("NewPassword", ErrorMessage = "Xác nhận mật khẩu không đúng")]
        public string ConfirmNewPassword { get; set; }
    }
}
