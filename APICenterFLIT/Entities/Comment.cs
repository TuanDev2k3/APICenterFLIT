using System;
using System.Collections.Generic;

namespace APICenterFlit.Entities;

public partial class Comment
{
    public int Id { get; set; }

    public int AccountId { get; set; }

    public int NewId { get; set; }

    public string Detail { get; set; } = null!;

    public int Status { get; set; }

    public DateTime CommentAt { get; set; }

    public DateTime UpdateAt { get; set; }

    public int UpdateBy { get; set; }

    public virtual Account Account { get; set; } = null!;

    public virtual News New { get; set; } = null!;
}
