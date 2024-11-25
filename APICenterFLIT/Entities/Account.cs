using System;
using System.Collections.Generic;

namespace APICenterFlit.Entities;

public partial class Account
{
    public int Id { get; set; }

    public string Fullname { get; set; } = null!;

    public string? UserName { get; set; }

    public string Password { get; set; } = null!;

    public string Email { get; set; } = null!;

    public int AccountType { get; set; }

    public int Status { get; set; }

    public string? Phone { get; set; }

    public int? AddressId { get; set; }

    public int ImageId { get; set; }

    public DateTime CreateAt { get; set; }

    public int CreateBy { get; set; }

    public DateTime UpdateAt { get; set; }

    public int UpdateBy { get; set; }

    public virtual ICollection<AccessLog> AccessLogs { get; set; } = new List<AccessLog>();

    public virtual AccountType AccountTypeNavigation { get; set; } = null!;

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual Image Image { get; set; } = null!;
}
