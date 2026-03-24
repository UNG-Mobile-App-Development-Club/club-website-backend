using System;
using System.Collections.Generic;

namespace CodeHawksApi.Models;

public partial class Event
{
    public int EventId { get; set; }

    public string EventName { get; set; } = null!;

    public string? EventDesc { get; set; }

    public string? EventPicUrl { get; set; }

    public DateTime EventDate { get; set; }

    public virtual ICollection<Member> Usernames { get; set; } = new List<Member>();
}
