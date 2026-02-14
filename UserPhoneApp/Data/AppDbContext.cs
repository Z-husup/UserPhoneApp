using Microsoft.EntityFrameworkCore;
using UserPhoneApp.Models;

namespace UserPhoneApp.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    /*
     * Database includes two tables:
     * Users
     * Phones
     */
    public DbSet<User> Users => Set<User>();
    public DbSet<Phone> Phones => Set<Phone>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Validation for unique Emails
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        // Validation for unique Phone numbers
        modelBuilder.Entity<Phone>()
            .HasIndex(p => p.PhoneNumber)
            .IsUnique();
        
        /*
         * ONE TO MANY Relationship - User to Phone
         * Delete behaviour: CASCADE
         */
        modelBuilder.Entity<Phone>()
            .HasOne(p => p.User)
            .WithMany(u => u.Phones)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }

}