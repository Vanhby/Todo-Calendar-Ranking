using System;
using System.Collections.Generic;

namespace StreakHub.API.Models_Generated;

public partial class Share
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public DateOnly TargetDate { get; set; }

    public string ShareCode { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Star> Stars { get; set; } = new List<Star>();

    public virtual User User { get; set; } = null!;

    public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();
}
