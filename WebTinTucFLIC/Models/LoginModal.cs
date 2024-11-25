using System.ComponentModel.DataAnnotations;

namespace WebTinTucFLIC.Models
{
    public class LoginModal
    {
        [Required(ErrorMessage = "Không được để trống")]
        [MaxLength(20)]
        public string UserName { get; set; } = null!;
        [Required(ErrorMessage = "Không được để trống")]
        [MaxLength(100)]
        public string Password { get; set; } = null!;
    }
}
