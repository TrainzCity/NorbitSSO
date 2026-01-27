using System;
using System.Collections.Generic;

namespace NorbitApi.Model;

public partial class RequestType
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Log> Logs { get; set; } = new List<Log>();
}
