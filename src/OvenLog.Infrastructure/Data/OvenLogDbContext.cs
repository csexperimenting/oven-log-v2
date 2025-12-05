using Microsoft.EntityFrameworkCore;
using OvenLog.Domain.Entities;

namespace OvenLog.Infrastructure.Data;

public class OvenLogDbContext : DbContext
{
    public OvenLogDbContext(DbContextOptions<OvenLogDbContext> options) : base(options)
    {
    }

    public DbSet<Location> Locations => Set<Location>();
    public DbSet<BoxType> BoxTypes => Set<BoxType>();
    public DbSet<Manufacturer> Manufacturers => Set<Manufacturer>();
    public DbSet<Model> Models => Set<Model>();
    public DbSet<Box> Boxes => Set<Box>();
    public DbSet<Part> Parts => Set<Part>();
    public DbSet<Trak> Traks => Set<Trak>();
    public DbSet<User> Users => Set<User>();
    public DbSet<UserAlias> UserAliases => Set<UserAlias>();
    public DbSet<Application> Applications => Set<Application>();
    public DbSet<StandardTime> StandardTimes => Set<StandardTime>();
    public DbSet<OvenEvent> OvenEvents => Set<OvenEvent>();
    public DbSet<OnEvent> OnEvents => Set<OnEvent>();
    public DbSet<UserOvenSelection> UserOvenSelections => Set<UserOvenSelection>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Location
        modelBuilder.Entity<Location>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
        });

        // BoxType
        modelBuilder.Entity<BoxType>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
        });

        // Manufacturer
        modelBuilder.Entity<Manufacturer>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
        });

        // Model
        modelBuilder.Entity<Model>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.HasOne(e => e.Manufacturer)
                .WithMany(m => m.Models)
                .HasForeignKey(e => e.ManufacturerId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Box
        modelBuilder.Entity<Box>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ToolId).IsRequired().HasMaxLength(50);
            entity.Property(e => e.TemperatureScale).HasMaxLength(10);
            
            entity.HasOne(e => e.Location)
                .WithMany(l => l.Boxes)
                .HasForeignKey(e => e.LocationId)
                .OnDelete(DeleteBehavior.Restrict);
            
            entity.HasOne(e => e.Model)
                .WithMany(m => m.Boxes)
                .HasForeignKey(e => e.ModelId)
                .OnDelete(DeleteBehavior.Restrict);
            
            entity.HasOne(e => e.BoxType)
                .WithMany(t => t.Boxes)
                .HasForeignKey(e => e.BoxTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Part
        modelBuilder.Entity<Part>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.PartNumber).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(500);
        });

        // Trak
        modelBuilder.Entity<Trak>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TrakId).IsRequired().HasMaxLength(50);
            entity.Property(e => e.SerialNumber).HasMaxLength(50);
            entity.Property(e => e.WorkOrder).HasMaxLength(50);
            
            entity.HasOne(e => e.Part)
                .WithMany(p => p.Traks)
                .HasForeignKey(e => e.PartId)
                .OnDelete(DeleteBehavior.SetNull);
            
            entity.HasIndex(e => e.TrakId);
        });

        // User
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(50);
            entity.Property(e => e.MiddleName).HasMaxLength(50);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Badge).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Login).IsRequired().HasMaxLength(100);
            
            entity.HasOne(e => e.DedicatedBox)
                .WithMany()
                .HasForeignKey(e => e.DedicatedBoxId)
                .OnDelete(DeleteBehavior.SetNull);
            
            entity.HasIndex(e => e.Badge);
            entity.HasIndex(e => e.Login);
        });

        // UserAlias
        modelBuilder.Entity<UserAlias>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Alias).IsRequired().HasMaxLength(100);
            entity.Property(e => e.UserName).IsRequired().HasMaxLength(100);
            
            entity.HasOne(e => e.User)
                .WithMany(u => u.Aliases)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Application
        modelBuilder.Entity<Application>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Barcode).HasMaxLength(50);
        });

        // StandardTime
        modelBuilder.Entity<StandardTime>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Barcode).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(100);
        });

        // OvenEvent
        modelBuilder.Entity<OvenEvent>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Note).HasMaxLength(1000);
            
            entity.HasOne(e => e.Box)
                .WithMany(b => b.Events)
                .HasForeignKey(e => e.BoxId)
                .OnDelete(DeleteBehavior.Restrict);
            
            entity.HasOne(e => e.UserIn)
                .WithMany(u => u.EventsIn)
                .HasForeignKey(e => e.UserInId)
                .OnDelete(DeleteBehavior.Restrict);
            
            entity.HasOne(e => e.UserOut)
                .WithMany(u => u.EventsOut)
                .HasForeignKey(e => e.UserOutId)
                .OnDelete(DeleteBehavior.Restrict);
            
            entity.HasOne(e => e.Trak)
                .WithMany(t => t.Events)
                .HasForeignKey(e => e.TrakId)
                .OnDelete(DeleteBehavior.Restrict);
            
            entity.HasOne(e => e.Application)
                .WithMany(a => a.Events)
                .HasForeignKey(e => e.ApplicationId)
                .OnDelete(DeleteBehavior.SetNull);
            
            entity.HasIndex(e => e.DateIn);
            entity.HasIndex(e => e.DateOut);
        });

        // OnEvent
        modelBuilder.Entity<OnEvent>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.HasOne(e => e.Box)
                .WithMany(b => b.OnEvents)
                .HasForeignKey(e => e.BoxId)
                .OnDelete(DeleteBehavior.Restrict);
            
            entity.HasOne(e => e.User)
                .WithMany(u => u.OnEvents)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // UserOvenSelection
        modelBuilder.Entity<UserOvenSelection>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(e => e.Box)
                .WithMany()
                .HasForeignKey(e => e.BoxId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasIndex(e => new { e.UserId, e.BoxId }).IsUnique();
        });
    }
}
