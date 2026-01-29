using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace NorbitApi.Model;

public partial class NorbitBaseContext : DbContext
{
    public NorbitBaseContext()
    {
    }

    public NorbitBaseContext(DbContextOptions<NorbitBaseContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AuthStatus> AuthStatuses { get; set; }

    public virtual DbSet<Log> Logs { get; set; }

    public virtual DbSet<RequestType> RequestTypes { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=sql-server;Database=NorbitBase;User Id=ituser09;Password=r3tz4Jev;Encrypt=False").UseLazyLoadingProxies();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AuthStatus>(entity =>
        {
            entity.ToTable("AuthStatus");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Name).HasMaxLength(120);
        });

        modelBuilder.Entity<Log>(entity =>
        {
            entity.ToTable("Log");

            entity.Property(e => e.Time).HasColumnType("datetime");

            entity.HasOne(d => d.Status).WithMany(p => p.Logs)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Log_AuthStatus");

            entity.HasOne(d => d.Type).WithMany(p => p.Logs)
                .HasForeignKey(d => d.TypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Log_RequestType");

            entity.HasOne(d => d.User).WithMany(p => p.Logs)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Log_User");
        });

        modelBuilder.Entity<RequestType>(entity =>
        {
            entity.ToTable("RequestType");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Name).HasMaxLength(200);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Uuid);

            entity.ToTable("User");

            entity.Property(e => e.Uuid)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("UUID");
            entity.Property(e => e.Email).HasMaxLength(120);
            entity.Property(e => e.Login).HasMaxLength(25);
            entity.Property(e => e.Name).HasMaxLength(25);
            entity.Property(e => e.Patronymic).HasMaxLength(30);
            entity.Property(e => e.Phone)
                .HasMaxLength(12)
                .IsFixedLength();
            entity.Property(e => e.Surname).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
