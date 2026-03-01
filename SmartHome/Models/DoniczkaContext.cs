using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace SmartHome.Models;

public partial class DoniczkaContext : DbContext
{
    public DoniczkaContext()
    {
    }

    public DoniczkaContext(DbContextOptions<DoniczkaContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Api1> Api1s { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=51.68.143.184;Initial Catalog=Doniczka;Persist Security Info=True;User ID=sa;Password=Ljckia1234.; TrustServerCertificate = true");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Api1>(entity =>
        {
            entity.HasKey(e => e.ApiId);

            entity.ToTable("API1");

            entity.Property(e => e.ApiId).HasColumnName("api_Id");
            entity.Property(e => e.ApiName)
                .HasMaxLength(256)
                .HasColumnName("api_Name");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UsrId);

            entity.Property(e => e.UsrId).HasColumnName("usr_ID");
            entity.Property(e => e.UsrDate)
                .HasColumnType("datetime")
                .HasColumnName("usr_date");
            entity.Property(e => e.UsrLogin)
                .HasMaxLength(50)
                .HasColumnName("usr_Login");
            entity.Property(e => e.UsrPassword)
                .HasMaxLength(200)
                .HasColumnName("usr_Password");

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

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
