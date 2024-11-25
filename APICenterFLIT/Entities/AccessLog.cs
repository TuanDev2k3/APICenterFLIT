using System;
using System.Collections.Generic;

namespace APICenterFlit.Entities;

public partial class AccessLog
{
    public int Id { get; set; }

    public int AccountId { get; set; }

    public string Description { get; set; } = null!;

    public string DeviceName { get; set; } = null!;

    public string BrowersName { get; set; } = null!;

    public DateTime Timer { get; set; }

    public string IpAddress { get; set; } = null!;

    public virtual Account Account { get; set; } = null!;
}
