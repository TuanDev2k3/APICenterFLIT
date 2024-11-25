using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace APICenterFlit.Models
{
    public class LoginModal
    {
        [Required]
        [MaxLength(20)]
        public string UserName { get; set; } = null!;
        [Required]
        [MaxLength(100)]
        public string Password { get; set; } = null!;
    }
}