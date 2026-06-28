using System;
using System.Collections.Generic;

namespace StreakHub.API.Models_Generated;

public partial class User
{
    public int UserId { get; set; }

    public string Username { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string AvatarUrl { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Code { get; set; }

    public TimeOnly? DndStart { get; set; }

    public TimeOnly? DndEnd { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<Share> Shares { get; set; } = new List<Share>();

    public virtual ICollection<Star> Stars { get; set; } = new List<Star>();

    public virtual ICollection<Todo> Todos { get; set; } = new List<Todo>();
}
