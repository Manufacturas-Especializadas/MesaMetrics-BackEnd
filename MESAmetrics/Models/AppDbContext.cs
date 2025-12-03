using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace MESAmetrics.Models;

public partial class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<RealTime> RealTime { get; set; }

    public virtual DbSet<RealTimeTags> RealTimeTags { get; set; }

    public virtual DbSet<Shifts> Shifts { get; set; }

    public virtual DbSet<Tags> Tags { get; set; }

    public virtual DbSet<Telemetry> Telemetry { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RealTime>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__RealTime__3214EC07B0D74A55");

            entity.Property(e => e.Availability).HasColumnName("availability");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("createdAt");
            entity.Property(e => e.EndTime).HasColumnName("endTime");
            entity.Property(e => e.EvenTime).HasColumnName("evenTime");
            entity.Property(e => e.ProductionTime).HasColumnName("productionTime");
            entity.Property(e => e.ShiftId).HasColumnName("shiftId");
            entity.Property(e => e.StartTime).HasColumnName("startTime");
            entity.Property(e => e.Strikes).HasColumnName("strikes");
            entity.Property(e => e.Title)
                .IsUnicode(false)
                .HasColumnName("title");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updatedAt");

            entity.HasOne(d => d.Shift).WithMany(p => p.RealTime)
                .HasForeignKey(d => d.ShiftId)
                .HasConstraintName("FK__RealTime__update__534D60F1");
        });

        modelBuilder.Entity<RealTimeTags>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__RealTime__3214EC07E0369BF1");

            entity.HasIndex(e => new { e.RealTimeId, e.TagId }, "UQ__RealTime__2CD3530C0FDBE38F").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("createdAt");
            entity.Property(e => e.RealTimeId).HasColumnName("realTimeId");
            entity.Property(e => e.TagId).HasColumnName("tagId");

            entity.HasOne(d => d.RealTime).WithMany(p => p.RealTimeTags)
                .HasForeignKey(d => d.RealTimeId)
                .HasConstraintName("FK__RealTimeT__realT__5BE2A6F2");

            entity.HasOne(d => d.Tag).WithMany(p => p.RealTimeTags)
                .HasForeignKey(d => d.TagId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__RealTimeT__tagId__5CD6CB2B");
        });

        modelBuilder.Entity<Shifts>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Shifts__3214EC07F9495D78");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("createdAt");
            entity.Property(e => e.ShiftName)
                .HasMaxLength(70)
                .IsUnicode(false)
                .HasColumnName("shiftName");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updatedAt");
        });

        modelBuilder.Entity<Tags>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Tags__3214EC07B967EEA2");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("createdAt");
            entity.Property(e => e.TagsName)
                .IsUnicode(false)
                .HasColumnName("tagsName");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updatedAt");
        });

        modelBuilder.Entity<Telemetry>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Telemetr__3214EC07D96C9A71");

            entity.Property(e => e.Active).HasColumnName("active");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("createdAt");
            entity.Property(e => e.CycleCount).HasColumnName("cycleCount");
            entity.Property(e => e.MachineId).HasColumnName("machineId");
            entity.Property(e => e.MessageId).HasColumnName("messageId");
            entity.Property(e => e.RealTimeId).HasColumnName("realTimeId");
            entity.Property(e => e.StopButton).HasColumnName("stopButton");

            entity.HasOne(d => d.RealTime).WithMany(p => p.Telemetry)
                .HasForeignKey(d => d.RealTimeId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__Telemetry__creat__571DF1D5");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}