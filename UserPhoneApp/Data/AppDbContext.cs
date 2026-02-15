using Microsoft.EntityFrameworkCore;
using UserPhoneApp.Models;

namespace UserPhoneApp.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Phone> Phones => Set<Phone>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<Phone>()
            .HasIndex(p => p.PhoneNumber)
            .IsUnique();
        
        modelBuilder.Entity<Phone>()
            .HasOne(p => p.User)
            .WithMany(u => u.Phones)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}