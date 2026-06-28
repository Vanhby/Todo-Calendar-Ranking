using System;
using System.Collections.Generic;

namespace StreakHub.API.Models_Generated;

public partial class Reminder
{
    public int Id { get; set; }

    public int TaskId { get; set; }

    public DateTime NotifyTime { get; set; }

    public bool IsSent { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Notification? Notification { get; set; }

    public virtual Todo Task { get; set; } = null!;
}
