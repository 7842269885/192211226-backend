using Microsoft.EntityFrameworkCore;
using GrowsmartAPI.Models;

namespace GrowsmartAPI.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Profile> Profiles { get; set; }
    public DbSet<Plant> Plants { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<PlantSpecies> Species { get; set; }
    public DbSet<CommonDisease> CommonDiseases { get; set; }
    public DbSet<RawIdentificationLog> IdentificationLogs { get; set; }
    public DbSet<ForgotPasswordOtp> PasswordResetOtps { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Ensure email uniqueness
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        // 1-to-1 relationship User -> Profile
        modelBuilder.Entity<User>()
            .HasOne(u => u.Profile)
            .WithOne(p => p.User)
            .HasForeignKey<Profile>(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // 1-to-Many relationship User -> Plants
        modelBuilder.Entity<User>()
            .HasMany(u => u.Plants)
            .WithOne(p => p.User)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // 1-to-Many relationship User -> Notifications
        modelBuilder.Entity<User>()
            .HasMany(u => u.Notifications)
            .WithOne(n => n.User)
            .HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
