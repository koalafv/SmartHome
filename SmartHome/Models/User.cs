using System;
using System.Collections.Generic;

namespace SmartHome.Models;

public partial class User
{
    public int UsrId { get; set; }

    public string UsrLogin { get; set; } = null!;

    public string UsrPassword { get; set; } = null!;

    public DateTime UsrDate { get; set; }

    public virtual ICollection<Api1> UsrrApis { get; set; } = new List<Api1>();
}
