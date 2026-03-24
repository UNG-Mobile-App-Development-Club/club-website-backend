using System;
using System.Collections.Generic;

namespace CodeHawksApi.Models;

public partial class Minor
{
    public string MinorName { get; set; } = null!;

    public virtual ICollection<Member> Usernames { get; set; } = new List<Member>();
}
