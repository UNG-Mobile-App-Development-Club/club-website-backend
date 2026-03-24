using System;
using System.Collections.Generic;

namespace CodeHawksApi.Models;

public partial class Team
{
    public int TeamId { get; set; }

    public string TeamName { get; set; } = null!;

    public string? TeamDesc { get; set; }

    public int? MaxPeople { get; set; }

    public string? TeamPictureUrl { get; set; }

    public virtual ICollection<Member> Usernames { get; set; } = new List<Member>();
}
