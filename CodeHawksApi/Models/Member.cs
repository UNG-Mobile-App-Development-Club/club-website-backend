using System;
using System.Collections.Generic;

namespace CodeHawksApi.Models;

public partial class Member
{
    public string Username { get; set; } = null!;

    public string Fullname { get; set; } = null!;

    public string Passwordhash { get; set; } = null!;

    public string? Profilepicurl { get; set; }

    public string Email { get; set; } = null!;

    public string? Bio { get; set; }

    public bool is_verified { get; set; }

    public string? Tempcode { get; set; }

    public string? Linkedin {get; set;}

    public string? Github {get; set;}

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();

    public virtual ICollection<Major> MajorNames { get; set; } = new List<Major>();

    public virtual ICollection<Minor> MinorNames { get; set; } = new List<Minor>();

    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();

    public virtual ICollection<Team> Teams { get; set; } = new List<Team>();
}
