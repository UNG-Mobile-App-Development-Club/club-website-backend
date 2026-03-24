using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System;

namespace CodeHawksApi.Models;

public partial class ClubDataContext : DbContext
{
    public ClubDataContext()
    {
    }

    public ClubDataContext(DbContextOptions<ClubDataContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Event> Events { get; set; }

    public virtual DbSet<Major> Majors { get; set; }

    public virtual DbSet<Member> Members { get; set; }

    public virtual DbSet<Minor> Minors { get; set; }

    public virtual DbSet<Project> Projects { get; set; }

    public virtual DbSet<Team> Teams { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        
         if (optionsBuilder.IsConfigured) return;

    var cs = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");
    if (!string.IsNullOrWhiteSpace(cs))
    {
        optionsBuilder.UseNpgsql(cs);
    }
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasKey(e => e.EventId).HasName("events_pkey");

            entity.ToTable("events");

            entity.Property(e => e.EventId).HasColumnName("event_id");
            entity.Property(e => e.EventDate).HasColumnName("event_date");
            entity.Property(e => e.EventDesc).HasColumnName("event_desc");
            entity.Property(e => e.EventName)
                .HasMaxLength(100)
                .HasColumnName("event_name");
            entity.Property(e => e.EventPicUrl)
                .HasMaxLength(255)
                .HasColumnName("event_pic_url");
        });

        modelBuilder.Entity<Major>(entity =>
        {
            entity.HasKey(e => e.MajorName).HasName("major_pkey");

            entity.ToTable("major");

            entity.Property(e => e.MajorName)
                .HasMaxLength(100)
                .HasColumnName("major_name");
        });

        modelBuilder.Entity<Member>(entity =>
        {
            entity.HasKey(e => e.Username).HasName("member_pkey");

            entity.ToTable("member");

            entity.HasIndex(e => e.Email, "member_email_key").IsUnique();

            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .HasColumnName("username");
            entity.Property(e => e.Bio)
                .HasMaxLength(255)
                .HasColumnName("bio");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.Fullname)
                .HasMaxLength(100)
                .HasColumnName("fullname");
            entity.Property(e => e.Passwordhash)
                .HasMaxLength(255)
                .HasColumnName("passwordhash");
            entity.Property(e => e.Profilepicurl)
                .HasMaxLength(255)
                .HasColumnName("profilepicurl");

            entity.HasMany(d => d.Events).WithMany(p => p.Usernames)
                .UsingEntity<Dictionary<string, object>>(
                    "EventMemberList",
                    r => r.HasOne<Event>().WithMany()
                        .HasForeignKey("EventId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("event_member_list_event_id_fkey"),
                    l => l.HasOne<Member>().WithMany()
                        .HasForeignKey("Username")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("event_member_list_username_fkey"),
                    j =>
                    {
                        j.HasKey("Username", "EventId").HasName("event_member_list_pkey");
                        j.ToTable("event_member_list");
                        j.IndexerProperty<string>("Username")
                            .HasMaxLength(50)
                            .HasColumnName("username");
                        j.IndexerProperty<int>("EventId").HasColumnName("event_id");
                    });

            entity.HasMany(d => d.MajorNames).WithMany(p => p.Usernames)
                .UsingEntity<Dictionary<string, object>>(
                    "MemberMajorList",
                    r => r.HasOne<Major>().WithMany()
                        .HasForeignKey("MajorName")
                        .HasConstraintName("member_major_list_major_name_fkey"),
                    l => l.HasOne<Member>().WithMany()
                        .HasForeignKey("Username")
                        .HasConstraintName("member_major_list_username_fkey"),
                    j =>
                    {
                        j.HasKey("Username", "MajorName").HasName("member_major_list_pkey");
                        j.ToTable("member_major_list");
                        j.IndexerProperty<string>("Username")
                            .HasMaxLength(50)
                            .HasColumnName("username");
                        j.IndexerProperty<string>("MajorName")
                            .HasMaxLength(100)
                            .HasColumnName("major_name");
                    });

            entity.HasMany(d => d.MinorNames).WithMany(p => p.Usernames)
                .UsingEntity<Dictionary<string, object>>(
                    "MemberMinorList",
                    r => r.HasOne<Minor>().WithMany()
                        .HasForeignKey("MinorName")
                        .HasConstraintName("member_minor_list_minor_name_fkey"),
                    l => l.HasOne<Member>().WithMany()
                        .HasForeignKey("Username")
                        .HasConstraintName("member_minor_list_username_fkey"),
                    j =>
                    {
                        j.HasKey("Username", "MinorName").HasName("member_minor_list_pkey");
                        j.ToTable("member_minor_list");
                        j.IndexerProperty<string>("Username")
                            .HasMaxLength(50)
                            .HasColumnName("username");
                        j.IndexerProperty<string>("MinorName")
                            .HasMaxLength(100)
                            .HasColumnName("minor_name");
                    });

            entity.HasMany(d => d.Projects).WithMany(p => p.Usernames)
                .UsingEntity<Dictionary<string, object>>(
                    "ProjectMemberList",
                    r => r.HasOne<Project>().WithMany()
                        .HasForeignKey("ProjectId")
                        .HasConstraintName("project_member_list_project_id_fkey"),
                    l => l.HasOne<Member>().WithMany()
                        .HasForeignKey("Username")
                        .HasConstraintName("project_member_list_username_fkey"),
                    j =>
                    {
                        j.HasKey("Username", "ProjectId").HasName("project_member_list_pkey");
                        j.ToTable("project_member_list");
                        j.IndexerProperty<string>("Username")
                            .HasMaxLength(50)
                            .HasColumnName("username");
                        j.IndexerProperty<int>("ProjectId").HasColumnName("project_id");
                    });

            entity.HasMany(d => d.Teams).WithMany(p => p.Usernames)
                .UsingEntity<Dictionary<string, object>>(
                    "TeamMemberList",
                    r => r.HasOne<Team>().WithMany()
                        .HasForeignKey("TeamId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("team_member_list_team_id_fkey"),
                    l => l.HasOne<Member>().WithMany()
                        .HasForeignKey("Username")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("team_member_list_username_fkey"),
                    j =>
                    {
                        j.HasKey("Username", "TeamId").HasName("team_member_list_pkey");
                        j.ToTable("team_member_list");
                        j.IndexerProperty<string>("Username")
                            .HasMaxLength(50)
                            .HasColumnName("username");
                        j.IndexerProperty<int>("TeamId").HasColumnName("team_id");
                    });
        });

        modelBuilder.Entity<Minor>(entity =>
        {
            entity.HasKey(e => e.MinorName).HasName("minor_pkey");

            entity.ToTable("minor");

            entity.Property(e => e.MinorName)
                .HasMaxLength(100)
                .HasColumnName("minor_name");
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.ProjectId).HasName("project_pkey");

            entity.ToTable("project");

            entity.Property(e => e.ProjectId).HasColumnName("project_id");
            entity.Property(e => e.ProjectDesc).HasColumnName("project_desc");
            entity.Property(e => e.ProjectName)
                .HasMaxLength(100)
                .HasColumnName("project_name");
            entity.Property(e => e.ProjectPicsUrl)
                .HasMaxLength(255)
                .HasColumnName("project_pics_url");
            entity.Property(e => e.RepoLink)
                .HasMaxLength(255)
                .HasColumnName("repo_link");
        });

        modelBuilder.Entity<Team>(entity =>
        {
            entity.HasKey(e => e.TeamId).HasName("team_pkey");

            entity.ToTable("team");

            entity.Property(e => e.TeamId).HasColumnName("team_id");
            entity.Property(e => e.MaxPeople).HasColumnName("max_people");
            entity.Property(e => e.TeamDesc).HasColumnName("team_desc");
            entity.Property(e => e.TeamName)
                .HasMaxLength(100)
                .HasColumnName("team_name");
            entity.Property(e => e.TeamPictureUrl)
                .HasMaxLength(255)
                .HasColumnName("team_picture_url");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
