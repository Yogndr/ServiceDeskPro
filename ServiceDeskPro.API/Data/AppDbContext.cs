using Microsoft.EntityFrameworkCore;
using ServiceDeskPro.API.Models;

namespace ServiceDeskPro.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }

    public DbSet<Incident> Incidents { get; set; }

    public DbSet<Comment> Comments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Email must be unique for every user
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        // User who created/reported the incident
        modelBuilder.Entity<Incident>()
            .HasOne(i => i.CreatedBy)
            .WithMany(u => u.CreatedIncidents)
            .HasForeignKey(i => i.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);

        // Engineer assigned to the incident
        modelBuilder.Entity<Incident>()
            .HasOne(i => i.AssignedEngineer)
            .WithMany(u => u.AssignedIncidents)
            .HasForeignKey(i => i.AssignedEngineerId)
            .OnDelete(DeleteBehavior.Restrict);

        // User who wrote a comment
        modelBuilder.Entity<Comment>()
            .HasOne(c => c.User)
            .WithMany(u => u.Comments)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}