using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UserManagement.Models;

namespace UserManagement.Data;

public class DataContext : DbContext, IDataContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options) {}

    public DbSet<User> Users { get; set; } = null!; // null! silences nullability warnings; EF will initialize the DbSet.

    //Activity logs table
    public DbSet<ActivityLog> ActivityLogs { get; set; } = null!;

    public async Task<int> SaveChangesAsync()
    {
        return await base.SaveChangesAsync();
    }

    protected override void OnModelCreating(ModelBuilder model)
    {
        // User configuration
        model.Entity<User>(e =>
        {
            e.Property(x => x.Forename).HasMaxLength(100).IsRequired();
            e.Property(x => x.Surname).HasMaxLength(100).IsRequired();
            e.Property(x => x.Email).HasMaxLength(256).IsRequired();
            e.HasIndex(x => x.Email).IsUnique();
            e.Property(x => x.IsActive).IsRequired();
        });

        //ActivityLog configuration
        model.Entity<ActivityLog>(e =>
        {
            e.Property(x => x.Action).HasMaxLength(100).IsRequired();
            e.Property(x => x.UserName).HasMaxLength(200);
            e.Property(x => x.PerformedBy).HasMaxLength(200);
            e.Property(x => x.Details).HasMaxLength(1000);
            e.Property(x => x.Timestamp).IsRequired();

            e.HasIndex(x => x.UserId);
        });

        base.OnModelCreating(model);
    }
}
