using System;
using System.Collections.Generic;

namespace StreakHub.API.Models_Generated;

public partial class Tag
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Color { get; set; } = null!;

    public virtual ICollection<Share> Shares { get; set; } = new List<Share>();

    public virtual ICollection<Todo> Todos { get; set; } = new List<Todo>();
}
