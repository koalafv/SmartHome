using System;
using System.Collections.Generic;

namespace SmartHome.Models;

public partial class Api1
{
    public int ApiId { get; set; }

    public string? ApiName { get; set; }

    public virtual ICollection<User> UsrrUsrs { get; set; } = new List<User>();
}
