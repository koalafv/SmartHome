using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace SmartHome.Models;

public partial class ApplicationContext : DbContext
{
    public ApplicationContext ( )
    {
    }

    public ApplicationContext ( DbContextOptions<ApplicationContext> options )
        : base(options)
    {
    }

    public virtual DbSet<Api1> Api1s { get; set; }
    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<DeviceType> DeviceTypes { get; set; }
    public virtual DbSet<Device> Devices { get; set; }
    public virtual DbSet<DeviceSettings> DeviceSettings { get; set; }
    public virtual DbSet<SensorReading> SensorReadings { get; set; }
    public virtual DbSet<WeatherRecord> WeatherRecords { get; set; }
    protected override void OnModelCreating ( ModelBuilder modelBuilder )
    {
        modelBuilder.Entity<Api1>(entity =>
        {
            entity.HasKey(e => e.ApiId);
            entity.ToTable("API1");
            entity.Property(e => e.ApiId).HasColumnName("api_Id");
            entity.Property(e => e.ApiName).HasMaxLength(256).HasColumnName("api_Name");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UsrId);
            entity.Property(e => e.UsrId).HasColumnName("usr_ID");
            entity.Property(e => e.UsrDate).HasColumnType("datetime").HasColumnName("usr_date");
            entity.Property(e => e.UsrLogin).HasMaxLength(50).HasColumnName("usr_Login");
            entity.Property(e => e.UsrPassword).HasMaxLength(200).HasColumnName("usr_Password");

            entity.HasMany(d => d.UsrrApis).WithMany(p => p.UsrrUsrs)
                .UsingEntity<Dictionary<string, object>>(
                    "Api2",
                    r => r.HasOne<Api1>().WithMany().HasForeignKey("UsrrApiId"),
                    l => l.HasOne<User>().WithMany().HasForeignKey("UsrrUsrId"),
                    j =>
                    {
                        j.HasKey("UsrrUsrId", "UsrrApiId");
                        j.ToTable("API2");
                        j.IndexerProperty<int>("UsrrUsrId").HasColumnName("usrr_usr_Id");
                        j.IndexerProperty<int>("UsrrApiId").HasColumnName("usrr_api_Id");
                    });
        });

        modelBuilder.Entity<DeviceType>(entity =>
        {
            entity.HasKey(e => e.TypeId);
            entity.ToTable("DeviceTypes");
            entity.Property(e => e.TypeName).HasMaxLength(100);
        });

        modelBuilder.Entity<Device>(entity =>
        {
            entity.HasKey(e => e.DeviceId);
            entity.ToTable("Devices");
            entity.Property(e => e.MacAddress).HasMaxLength(50);
            entity.Property(e => e.CustomName).HasMaxLength(100);

            entity.HasOne(d => d.Type)
                  .WithMany(t => t.Devices)
                  .HasForeignKey(d => d.DeviceTypeId);

            entity.HasOne(d => d.Owner)
                  .WithMany()
                  .HasForeignKey(d => d.OwnerId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial ( ModelBuilder modelBuilder );
}