using System;
using System.Collections.Generic;

namespace APICenterFlit.Entities;

public partial class NewsType
{
    public int Id { get; set; }

    public int ParentId { get; set; }

    public string Title { get; set; } = null!;

    public string? TitleSlug { get; set; }

    public int ImageId { get; set; }

    public int Status { get; set; }

    public DateTime CreateAt { get; set; }

    public int CreateBy { get; set; }

    public DateTime UpdateAt { get; set; }

    public int UpdateBy { get; set; }

    public int? HomePage { get; set; }

    public virtual Image Image { get; set; } = null!;

    public virtual ICollection<News> News { get; set; } = new List<News>();
}
