using AlarmClock.Models;
using Microsoft.EntityFrameworkCore;

public class AlarmContext : DbContext
{
    public DbSet<Alarm> Alarms => Set<Alarm>();

    public AlarmContext()
    {
        // Automatically create database + tables if they don't exist
        Database.EnsureCreated();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlite("Data Source=alarms.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Alarm>().ToTable("Alarms");
        modelBuilder.Entity<Alarm>().HasKey(a => a.Id);
        modelBuilder.Entity<Alarm>()
            .Property(a => a.Enabled)
            .HasConversion<int>();
    }
}
