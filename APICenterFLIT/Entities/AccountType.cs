using System;
using System.Collections.Generic;

namespace APICenterFlit.Entities;

public partial class AccountType
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Code { get; set; } = null!;

    public int Status { get; set; }

    public DateTime CreateAt { get; set; }

    public int CreateBy { get; set; }

    public DateTime UpdateAt { get; set; }

    public int UpdateBy { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();
}
