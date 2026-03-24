using System;
using System.Collections.Generic;

namespace CodeHawksApi.Models;

public partial class Project
{
    public int ProjectId { get; set; }

    public string ProjectName { get; set; } = null!;

    public string? ProjectDesc { get; set; }

    public string? RepoLink { get; set; }

    public string? ProjectPicsUrl { get; set; }

    public virtual ICollection<Member> Usernames { get; set; } = new List<Member>();
}
