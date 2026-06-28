using System;
using System.Collections.Generic;

namespace StreakHub.API.Models_Generated;

public partial class Notification
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int TodoId { get; set; }

    public int ReminderId { get; set; }

    public string Title { get; set; } = null!;

    public string Message { get; set; } = null!;

    public bool IsRead { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Reminder Reminder { get; set; } = null!;

    public virtual Todo Todo { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
