using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ComplaintSystem.Domain.Entities;

namespace ComplaintSystem.Infrastructure.Data;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
}

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Category> Categories { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<Complaint> Complaints { get; set; }
    public DbSet<ComplaintLog> ComplaintLogs { get; set; }
    public DbSet<Feedback> Feedbacks { get; set; }
    public DbSet<Comment> Comments { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Comment>()
            .HasOne(c => c.Complaint)
            .WithMany(c => c.Comments)
            .HasForeignKey(c => c.ComplaintId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Complaint>()
            .HasOne(c => c.Category)
            .WithMany(c => c.Complaints)
            .HasForeignKey(c => c.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Complaint>()
            .HasOne(c => c.Department)
            .WithMany(d => d.Complaints)
            .HasForeignKey(c => c.DepartmentId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<ComplaintLog>()
            .HasOne(l => l.Complaint)
            .WithMany(c => c.Logs)
            .HasForeignKey(l => l.ComplaintId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.Entity<Feedback>()
            .HasOne(f => f.Complaint)
            .WithOne(c => c.Feedback)
            .HasForeignKey<Feedback>(f => f.ComplaintId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
