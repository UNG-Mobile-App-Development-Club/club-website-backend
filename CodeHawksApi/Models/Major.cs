using System;
using System.Collections.Generic;

namespace CodeHawksApi.Models;

public partial class Major
{
    public string MajorName { get; set; } = null!;

    public virtual ICollection<Member> Usernames { get; set; } = new List<Member>();
}
