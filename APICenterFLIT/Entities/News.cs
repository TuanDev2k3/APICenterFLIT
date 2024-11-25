using System;
using System.Collections.Generic;

namespace APICenterFlit.Entities;

public partial class News
{
    public int Id { get; set; }

    public int NewTypeId { get; set; }

    public string Title { get; set; } = null!;

    public string? TitleSlug { get; set; }

    public string? Description { get; set; }

    public string Detail { get; set; } = null!;

    public int ImageId { get; set; }

    public DateTime PublishAt { get; set; }

    public int? IsHot { get; set; }

    public int Status { get; set; }

    public DateTime CreateAt { get; set; }

    public int CreateBy { get; set; }

    public DateTime UpdateAt { get; set; }

    public int UpdateBy { get; set; }

    public int? HomePage { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual Image Image { get; set; } = null!;

    public virtual NewsType NewType { get; set; } = null!;
}
