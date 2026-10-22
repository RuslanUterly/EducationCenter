using Microsoft.EntityFrameworkCore;

namespace EducationCenter.Api.Domain.Context;

public class EducationDbContext : DbContext
{
    public EducationDbContext(DbContextOptions<EducationDbContext> options) : base(options) { }

    public DbSet<Course> Courses => Set<Course>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Course>(e =>
        {
            e.HasKey(c => c.Id);
            e.Property(c => c.Name).IsRequired().HasMaxLength(200);
            e.Property(c => c.Price).HasPrecision(18, 2);
        });

        modelBuilder.Entity<User>(e =>
        {
            e.HasKey(u => u.Id);
            e.Property(u => u.Name).IsRequired().HasMaxLength(100);
        });
    }
}

