using System;
using System.Collections.Generic;

namespace APICenterFlit.Entities;

public partial class Image
{
    public int Id { get; set; }

    public string? Description { get; set; }

    public string ImageUrl { get; set; } = null!;

    public int Status { get; set; }

    public DateTime CreateAt { get; set; }

    public int CreateBy { get; set; }

    public DateTime? UpdateAt { get; set; }

    public int? UpdateBy { get; set; }

    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();

    public virtual ICollection<News> News { get; set; } = new List<News>();

    public virtual ICollection<NewsType> NewsTypes { get; set; } = new List<NewsType>();
}
