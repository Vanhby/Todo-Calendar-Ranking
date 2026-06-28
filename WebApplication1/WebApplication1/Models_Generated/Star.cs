using System;
using System.Collections.Generic;

namespace StreakHub.API.Models_Generated;

public partial class Star
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int ShareId { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Share Share { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
