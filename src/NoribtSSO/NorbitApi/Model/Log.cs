using System;
using System.Collections.Generic;

namespace NorbitApi.Model;

public partial class Log
{
    public int Id { get; set; }

    public int StatusId { get; set; }

    public int TypeId { get; set; }

    public Guid? UserId { get; set; }

    public DateTime Time { get; set; }

    public virtual AuthStatus Status { get; set; } = null!;

    public virtual RequestType Type { get; set; } = null!;

    public virtual User? User { get; set; }
}
